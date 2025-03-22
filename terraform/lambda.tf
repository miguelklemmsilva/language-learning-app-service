resource "aws_lambda_permission" "allow_cognito_preSignUp" {
  statement_id  = "AllowPreSignUpExecutionFromCognito"
  action        = "lambda:InvokeFunction"
  function_name = aws_lambda_function.pre_sign_up.function_name
  principal     = "cognito-idp.amazonaws.com"
  source_arn    = aws_cognito_user_pool.user_pool.arn
}

data "aws_iam_policy_document" "lambda_policy" {
  statement {
    actions = [
      "dynamodb:PutItem",
      "dynamodb:GetItem",
      "dynamodb:UpdateItem",
      "dynamodb:Query",
      "dynamodb:DeleteItem",
      "secretsmanager:GetSecretValue",
    ]
    effect = "Allow"
    resources = [
      aws_dynamodb_table.users.arn, aws_dynamodb_table.user_languages.arn, aws_dynamodb_table.vocabulary.arn,
      aws_dynamodb_table.allowed_vocabulary.arn,  "${aws_dynamodb_table.allowed_vocabulary.arn}/index/*",
    ]
  }
}