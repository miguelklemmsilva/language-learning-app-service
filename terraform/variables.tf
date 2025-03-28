variable "api_routes" {
  type = list(object({
    path            = string
    http_method     = string
    lambda_function = string # Placeholder
  }))

  default = [
    {
      path            = "language"
      http_method     = "PUT"
      lambda_function = "UpdateLanguage"
    },
    {
      path            = "user"
      http_method     = "GET"
      lambda_function = "GetUser"
    },
    {
      path            = "languages"
      http_method     = "GET"
      lambda_function = "GetUserLanguages"
    },
    {
      path            = "language"
      http_method     = "DELETE"
      lambda_function = "RemoveLanguage"
    },
    {
      path            = "user"
      http_method     = "PUT"
      lambda_function = "UpdateUser"
    },
    {
      path            = "vocabulary"
      http_method     = "PUT"
      lambda_function = "AddVocabulary"
    },
    {
      path            = "vocabulary"
      http_method     = "GET"
      lambda_function = "GetVocabulary"
    },
    {
      path            = "vocabulary"
      http_method     = "DELETE"
      lambda_function = "RemoveVocabulary"
    },
    {
      path            = "generatesentences"
      http_method     = "GET"
      lambda_function = "GenerateSentences"
    },
    {
      path            = "verifysentence"
      http_method     = "POST"
      lambda_function = "VerifySentence"
    },
    {
      path            = "finishlesson"
      http_method     = "POST"
      lambda_function = "FinishLesson"
    },
    {
      path            = "issuetoken"
      http_method     = "GET"
      lambda_function = "IssueToken"
    },
    {
      path            = "categories"
      http_method     = "GET"
      lambda_function = "GetCategories"
    }
  ]
}

variable SPEECH_KEY {}
variable "CHAT_GPT_KEY" {}
variable "TRANSLATOR_KEY" {}
variable "AWS_REGION" {
  type = string
  default = "eu-west-2"
}
