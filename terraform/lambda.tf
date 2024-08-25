data "aws_lambda_function" "pre_sign_up" {
  function_name = "language-learning-app-PreSignUpFunction-rW5Ei1Q7GTV7"
}

data "aws_iam_role" "pre_sign_up_lambda_role" {
  name = "language-learning-app-PreSignUpFunctionRole-TNzIgUAgMi5J"
}

data "aws_lambda_function" "update_language" {
  function_name = "language-learning-app-UpdateLanguageFunction-5TgZJhdi9JeJ"
}

data "aws_iam_role" "update_language_lambda_role" {
  name = "language-learning-app-UpdateLanguageFunctionRole-mX09jyVe3g0u"
}

data "aws_lambda_function" "get_user" {
  function_name = "language-learning-app-GetUserFunction-Rs8DA5kfSGWI"
}

data "aws_iam_role" "get_user_lambda_role" {
  name = "language-learning-app-GetUserFunctionRole-8CQ9Z7ShPki9"
}

data "aws_lambda_function" "get_user_languages" {
  function_name = "language-learning-app-GetUserLanguagesFunction-16IBdpQXmreg"
}

data "aws_iam_role" "get_user_languages_lambda_role" {
  name = "language-learning-app-GetUserLanguagesFunctionRole-NigLu8zHX3Le"
}

data "aws_lambda_function" "remove_user_language" {
  function_name = "language-learning-app-RemoveUserLanguagesFunction-4VuvTwxd6gKo"
}

data "aws_iam_role" "remove_user_language_lambda_role" {
  name = "language-learning-app-RemoveUserLanguagesFunctionRo-MMo4Wb3bdD0X"
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
      "dynamodb:Query",
      "dynamodb:DeleteItem",
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

resource "aws_iam_role_policy_attachment" "attach_get_user_policy" {
  role       = data.aws_iam_role.get_user_lambda_role.name
  policy_arn = aws_iam_policy.lambda_policy.arn
}

resource "aws_iam_role_policy_attachment" "attach_get_user_languages_policy" {
  role       = data.aws_iam_role.get_user_languages_lambda_role.name
  policy_arn = aws_iam_policy.lambda_policy.arn
}

resource "aws_iam_role_policy_attachment" "attach_remove_user_language_policy" {
  role       = data.aws_iam_role.remove_user_language_lambda_role.name
  policy_arn = aws_iam_policy.lambda_policy.arn
}




