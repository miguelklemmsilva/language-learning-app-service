resource "aws_iam_role" "lambda_role" {
  for_each = {for route in var.api_routes : route.lambda_function => route}

  name = "role-${each.key}"
  assume_role_policy = jsonencode({
    Version = "2012-10-17",
    Statement = [
      {
        Action = "sts:AssumeRole",
        Effect = "Allow",
        Principal = {
          Service = "lambda.amazonaws.com"
        }
      }
    ]
  })
}

resource "aws_iam_role" "pre_sign_up_role" {
  name = "role-PreSignUp"
  assume_role_policy = jsonencode({
    Version = "2012-10-17",
    Statement = [
      {
        Action = "sts:AssumeRole",
        Effect = "Allow",
        Principal = {
          Service = "lambda.amazonaws.com"
        }
      }
    ]
  })
}

resource "aws_lambda_function" "api_route_functions" {
  for_each = {for route in var.api_routes : route.lambda_function => route}

  s3_bucket     = "polybara-artifacts"
  s3_object_version = data.aws_s3_object.lambda_artifact_object.version_id
  s3_key        = "Lambdas.zip"
  function_name = "polybara-${each.key}"
  role          = aws_iam_role.lambda_role[each.key].arn
  handler       = "bootstrap"
  runtime       = "dotnet8"

  environment {
    variables = {
      ANNOTATIONS_HANDLER = each.key
      SPEECH_KEY          = var.SPEECH_KEY
      TRANSLATOR_KEY      = var.TRANSLATOR_KEY
      CHAT_GPT_KEY        = var.CHAT_GPT_KEY
    }
  }
}

resource "aws_lambda_function" "pre_sign_up" {
  s3_bucket     = "polybara-artifacts"
  s3_key        = "Lambdas.zip"
  s3_object_version = data.aws_s3_object.lambda_artifact_object.version_id
  function_name = "polybara-PreSignUp"
  role          = aws_iam_role.pre_sign_up_role.arn
  handler       = "bootstrap"
  runtime       = "dotnet8"

  environment {
    variables = {
      ANNOTATIONS_HANDLER = "PreSignUpTrigger"
      SPEECH_KEY          = var.SPEECH_KEY
      TRANSLATOR_KEY      = var.TRANSLATOR_KEY
      CHAT_GPT_KEY        = var.CHAT_GPT_KEY
    }
  }
}

resource "aws_lambda_permission" "allow_cognito_preSignUp" {
  statement_id  = "AllowPreSignUpExecutionFromCognito"
  action        = "lambda:InvokeFunction"
  function_name = aws_lambda_function.pre_sign_up.function_name
  principal     = "cognito-idp.amazonaws.com"
  source_arn    = aws_cognito_user_pool.user_pool.arn
}

resource "aws_iam_role_policy" "lambda_policy" {
  for_each = {for route in var.api_routes : route.lambda_function => route}

  name = "${each.key}-policy"
  role = aws_iam_role.lambda_role[each.key].id

  policy = jsonencode({
    Version = "2012-10-17",
    Statement = [
      {
        Effect = "Allow",
        Action = [
          "dynamodb:PutItem",
          "dynamodb:GetItem",
          "dynamodb:UpdateItem",
          "dynamodb:Query",
          "dynamodb:DeleteItem",
        ],
        Resource = [
          aws_dynamodb_table.users.arn,
          aws_dynamodb_table.user_languages.arn,
          aws_dynamodb_table.vocabulary.arn,
          aws_dynamodb_table.allowed_vocabulary.arn,
          "${aws_dynamodb_table.allowed_vocabulary.arn}/index/*"
        ]
      }
    ]
  })
}

resource "aws_iam_role_policy" "pre_sign_up_policy" {
  name = "PreSignUp-policy"
  role = aws_iam_role.pre_sign_up_role.id

  policy = jsonencode({
    Version = "2012-10-17",
    Statement = [
      {
        Effect = "Allow",
        Action = [
          "dynamodb:PutItem",
          "dynamodb:GetItem",
          "dynamodb:UpdateItem",
          "dynamodb:Query",
          "dynamodb:DeleteItem",
        ],
        Resource = [
          aws_dynamodb_table.users.arn,
          aws_dynamodb_table.user_languages.arn,
          aws_dynamodb_table.vocabulary.arn,
          aws_dynamodb_table.allowed_vocabulary.arn,
          "${aws_dynamodb_table.allowed_vocabulary.arn}/index/*"
        ]
      }
    ]
  })
}