data "aws_lambda_function" "pre_sign_up" {
  function_name = "language-learning-app-PreSignUpFunction-c2EgeILvBscU"
}

data "aws_iam_role" "pre_sign_up_lambda_role" {
  name = "language-learning-app-PreSignUpFunctionRole-raE5RaEOALPy"
}

data "aws_lambda_function" "update_language" {
  function_name = "language-learning-app-UpdateLanguageFunction-8ZbQgE75EYww"
}

data "aws_iam_role" "update_language_lambda_role" {
  name = "language-learning-app-UpdateLanguageFunctionRole-foMTihm7lwFF"
}

data "aws_lambda_function" "get_user" {
  function_name = "language-learning-app-GetUserFunction-eV7uyp131nIH"
}

data "aws_iam_role" "get_user_lambda_role" {
  name = "language-learning-app-GetUserFunctionRole-IwNiUdLI7YBe"
}

data "aws_lambda_function" "get_user_languages" {
  function_name = "language-learning-app-GetUserLanguagesFunction-cyY1K7NGD85d"
}

data "aws_iam_role" "get_user_languages_lambda_role" {
  name = "language-learning-app-GetUserLanguagesFunctionRole-evm6Ncog26JI"
}

data "aws_lambda_function" "remove_user_language" {
  function_name = "language-learning-app-RemoveUserLanguageFunction-z6pFqwW00FDt"
}

data "aws_iam_role" "remove_user_language_lambda_role" {
  name = "language-learning-app-RemoveUserLanguageFunctionRol-QqlnVMcVOQ3c"
}

data "aws_lambda_function" "update_user" {
  function_name = "language-learning-app-UpdateUserFunction-dqqe1yoZUhIc"
}

data "aws_iam_role" "update_user_lambda_role" {
  name = "language-learning-app-UpdateUserFunctionRole-0R6yNgN9W7nA"
}

data "aws_lambda_function" "add_vocabulary" {
  function_name = "language-learning-app-AddVocabularyFunction-9x1jrghSzk1x"
}

data "aws_iam_role" "add_vocabulary_lambda_role" {
  name = "language-learning-app-AddVocabularyFunctionRole-5mG8GXpWAwN8"
}

data "aws_lambda_function" "get_vocabulary" {
  function_name = "language-learning-app-GetVocabularyFunction-76vbu0rDf9KK"
}

data "aws_iam_role" "get_vocabulary_lambda_role" {
  name = "language-learning-app-GetVocabularyFunctionRole-LzlmuEvqR3Vs"
}

data "aws_lambda_function" "remove_vocabulary" {
  function_name = "language-learning-app-RemoveVocabularyFunction-2zW4otmaNAsx"
}

data "aws_iam_role" "remove_vocabulary_lambda_role" {
  name = "language-learning-app-RemoveVocabularyFunctionRole-CLnFWW0IiJmc"
}

data "aws_lambda_function" "generate_sentences" {
  function_name = "language-learning-app-GenerateSentencesFunction-LS6YuQpN2xB5"
}

data "aws_iam_role" "generate_sentences_lambda_role" {
  name = "language-learning-app-GenerateSentencesFunctionRole-Jyqr1L3x6P0w"
}

data "aws_lambda_function" "verify_sentence" {
    function_name = "ServerlessHuggingFaceStack-verifysentence747B9F22-yKn0hnTZ80Wb"
}

data "aws_iam_role" "verify_sentence_lambda_role" {
    name = "ServerlessHuggingFaceStac-verifysentenceServiceRole-nvNUOABu3YCz"
}

data "aws_secretsmanager_secret" "chat_gpt_key" {
  name = "ChatGptKey"
}

data "aws_secretsmanager_secret" "translator_key" {
  name = "TranslatorKey"
}

data "aws_secretsmanager_secret" "speech_key" {
  name = "SpeechKey"
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
      "secretsmanager:GetSecretValue"
    ]
    effect = "Allow"
    resources = [
      aws_dynamodb_table.users.arn, aws_dynamodb_table.user_languages.arn, aws_dynamodb_table.vocabulary.arn,
      aws_dynamodb_table.allowed_vocabulary.arn, data.aws_secretsmanager_secret.chat_gpt_key.arn,
      data.aws_secretsmanager_secret.translator_key.arn, data.aws_secretsmanager_secret.speech_key.arn
    ]
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

resource "aws_iam_role_policy_attachment" "attach_add_vocabulary_policy" {
  role       = data.aws_iam_role.add_vocabulary_lambda_role.name
  policy_arn = aws_iam_policy.lambda_policy.arn
}

resource "aws_iam_role_policy_attachment" "attach_get_vocabulary_policy" {
  role       = data.aws_iam_role.get_vocabulary_lambda_role.name
  policy_arn = aws_iam_policy.lambda_policy.arn
}

resource "aws_iam_role_policy_attachment" "attach_remove_vocabulary_policy" {
  role       = data.aws_iam_role.remove_vocabulary_lambda_role.name
  policy_arn = aws_iam_policy.lambda_policy.arn
}

resource "aws_iam_role_policy_attachment" "attach_generate_sentences_policy" {
  role       = data.aws_iam_role.generate_sentences_lambda_role.name
  policy_arn = aws_iam_policy.lambda_policy.arn
}

