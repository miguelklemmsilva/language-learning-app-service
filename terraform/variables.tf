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
    }
  ]
}

locals {
  lambda_function_map = {
    "updatelanguage_function"     = data.aws_lambda_function.update_language.invoke_arn
    "getuser_function"            = data.aws_lambda_function.get_user.invoke_arn
    "getuserlanguages_function"   = data.aws_lambda_function.get_user_languages.invoke_arn
    "removeuserlanguage_function" = data.aws_lambda_function.remove_user_language.invoke_arn
    "updateuser_function"         = data.aws_lambda_function.update_user.invoke_arn
    "addvocabulary_function"      = data.aws_lambda_function.add_vocabulary.invoke_arn
    "getvocabulary_function"      = data.aws_lambda_function.get_vocabulary.invoke_arn
    "removevocabulary_function"   = data.aws_lambda_function.remove_vocabulary.invoke_arn
  }

  lambda_arn_map = {
    "updatelanguage_function"     = data.aws_lambda_function.update_language.arn
    "getuser_function"            = data.aws_lambda_function.get_user.arn
    "getuserlanguages_function"   = data.aws_lambda_function.get_user_languages.arn
    "removeuserlanguage_function" = data.aws_lambda_function.remove_user_language.arn
    "updateuser_function"         = data.aws_lambda_function.update_user.arn
    "addvocabulary_function"      = data.aws_lambda_function.add_vocabulary.arn
    "getvocabulary_function"      = data.aws_lambda_function.get_vocabulary.arn
    "removevocabulary_function"   = data.aws_lambda_function.remove_vocabulary.arn
  }
}