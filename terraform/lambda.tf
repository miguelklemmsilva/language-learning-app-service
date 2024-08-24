data "aws_lambda_function" "pre_sign_up" {
  function_name = "language-learning-app-PostSignUpFunction-DUcKyEq26HFf"
}

data "aws_iam_role" "pre_sign_up_lambda_role" {
  name = "language-learning-app-PostSignUpFunctionRole-3xD1b0sxS8fd"
}

data "aws_lambda_function" "update_language" {
  function_name = "language-learning-app-UpdateLanguageFunction-uhbdgUbTA1Z3"
}

data "aws_iam_role" "update_language_lambda_role" {
  name = "language-learning-app-UpdateLanguageFunctionRole-smYdUkOQmJoZ"
}

resource "aws_lambda_permission" "allow_cognito_preSignUp" {
  statement_id  = "AllowPreSignUpExecutionFromCognito"
  action        = "lambda:InvokeFunction"
  function_name = data.aws_lambda_function.pre_sign_up.function_name
  principal     = "cognito-idp.amazonaws.com"
  source_arn    = aws_cognito_user_pool.user_pool.arn
}

data "aws_iam_policy_document" "lambda_policy" {
  statement {
    actions = [
      "dynamodb:PutItem",
      "dynamodb:GetItem",
      "dynamodb:UpdateItem",
      "dynamodb:Query"
    ]
    effect = "Allow"
    resources = [aws_dynamodb_table.users.arn, aws_dynamodb_table.user_languages.arn]
  }
}

resource "aws_iam_policy" "lambda_policy" {
  name   = "LambdaPolicy"
  policy = data.aws_iam_policy_document.lambda_policy.json
}

resource "aws_iam_role_policy_attachment" "attach_pre_signup_policy" {
  role       = data.aws_iam_role.pre_sign_up_lambda_role.name
  policy_arn = aws_iam_policy.lambda_policy.arn
}

resource "aws_iam_role_policy_attachment" "attach_update_language_policy" {
  role       = data.aws_iam_role.update_language_lambda_role.name
  policy_arn = aws_iam_policy.lambda_policy.arn
}




