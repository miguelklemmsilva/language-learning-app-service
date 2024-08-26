data "aws_lambda_function" "pre_sign_up" {
  function_name = "language-learning-app-PreSignUpFunction-OnrppInklow4"
}

data "aws_iam_role" "pre_sign_up_lambda_role" {
  name = "language-learning-app-PreSignUpFunctionRole-gb6UaGhm4BC7"
}

data "aws_lambda_function" "update_language" {
  function_name = "language-learning-app-UpdateLanguageFunction-v3kSpmKMbhL6"
}

data "aws_iam_role" "update_language_lambda_role" {
  name = "language-learning-app-UpdateLanguageFunctionRole-iCQkaVKjyk4j"
}

data "aws_lambda_function" "get_user" {
  function_name = "language-learning-app-GetUserFunction-eV7uyp131nIH"
}

data "aws_iam_role" "get_user_lambda_role" {
  name = "language-learning-app-GetUserFunctionRole-IwNiUdLI7YBe"
}

data "aws_lambda_function" "get_user_languages" {
  function_name = "language-learning-app-GetUserLanguagesFunction-abAwGiwZX1RE"
}

data "aws_iam_role" "get_user_languages_lambda_role" {
  name = "language-learning-app-GetUserLanguagesFunctionRole-pdi6oUWPngKf"
}

data "aws_lambda_function" "remove_user_language" {
  function_name = "language-learning-app-RemoveUserLanguagesFunction-4VuvTwxd6gKo"
}

data "aws_iam_role" "remove_user_language_lambda_role" {
  name = "language-learning-app-RemoveUserLanguagesFunctionRo-MMo4Wb3bdD0X"
}

data "aws_lambda_function" "update_user" {
  function_name = "language-learning-app-UpdateUserFunction-Dhtse8Skce9e"
}

data "aws_iam_role" "update_user_lambda_role" {
  name = "language-learning-app-UpdateUserFunctionRole-UM2lqhDTTDcb"
}

data "aws_lambda_function" "add_vocabulary" {
  function_name = "language-learning-app-AddVocabularyFunction-ElLcgwkccldr"
}

data "aws_iam_role" "add_vocabulary_lambda_role" {
  name = "language-learning-app-AddVocabularyFunctionRole-OPjDaGDKqgpx"
}

data "aws_lambda_function" "get_vocabulary" {
  function_name = "language-learning-app-GetVocabularyFunction-TczttQO3m2uj"
}

data "aws_iam_role" "get_vocabulary_lambda_role" {
  name = "language-learning-app-GetVocabularyFunctionRole-7WCFbpg0NK5k"
}

data "aws_lambda_function" "remove_vocabulary" {
  function_name = "language-learning-app-RemoveVocabularyFunction-EcmGpNz2xSC2"
}

data "aws_iam_role" "remove_vocabulary_lambda_role" {
  name = "language-learning-app-RemoveVocabularyFunctionRole-F5r53XPzxikn"
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

resource "aws_iam_role_policy_attachment" "attach_update_user_policy" {
  role       = data.aws_iam_role.update_user_lambda_role.name
  policy_arn = aws_iam_policy.lambda_policy.arn
}



