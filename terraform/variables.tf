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
      lambda_function = "updatelanguage_function"
    },
    {
      path            = "user"
      http_method     = "GET"
      lambda_function = "getuser_function"
    },
    {
      path            = "languages"
      http_method     = "GET"
      lambda_function = "getuserlanguages_function"
    },
    {
      path            = "language"
      http_method     = "DELETE"
      lambda_function = "removeuserlanguage_function"
    },
    {
      path            = "user"
      http_method     = "PUT"
      lambda_function = "updateuser_function"
    },
    {
      path            = "vocabulary"
      http_method     = "PUT"
      lambda_function = "addvocabulary_function"
    },
    {
      path            = "vocabulary"
      http_method     = "GET"
      lambda_function = "getvocabulary_function"
    },
    {
      path            = "vocabulary"
      http_method     = "DELETE"
      lambda_function = "removevocabulary_function"
    },
    {
      path            = "generatesentences"
      http_method     = "GET"
      lambda_function = "generate_sentences_function"
    },
    {
      path            = "verifysentence"
      http_method     = "POST"
      lambda_function = "verify_sentence_function"
    },
    {
      path            = "finishlesson"
      http_method     = "POST"
      lambda_function = "finish_lesson_function"
    },
    {
      path            = "issuetoken"
      http_method     = "GET"
      lambda_function = "issue_token_function"
    },
    {
      path            = "categories"
      http_method     = "GET"
      lambda_function = "getcategories_function"
    }
  ]
}