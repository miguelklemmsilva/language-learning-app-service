variable "api_routes" {
  type = list(object({
    path            = string
    http_method     = string
    lambda_function = string # Placeholder
    authorization   = string
    authorizer      = bool
    enable_cors     = bool
  }))

  default = [
    {
      path            = "updatelanguage"
      http_method     = "POST"
      lambda_function = "updatelanguage_function" # Placeholder
      authorization   = "COGNITO_USER_POOLS"
      authorizer      = true
      enable_cors     = true
    },
    {
      path            = "getuser"
      http_method     = "GET"
      lambda_function = "getuser_function" # Placeholder
      authorization   = "COGNITO_USER_POOLS"
      authorizer      = true
      enable_cors     = true
    },
    {
      path            = "getuserlanguages"
      http_method     = "GET"
      lambda_function = "getuserlanguages_function" # Placeholder
      authorization   = "COGNITO_USER_POOLS"
      authorizer      = true
      enable_cors     = true
    },
    {
      path            = "removeuserlanguage"
      http_method     = "DELETE"
      lambda_function = "removeuserlanguage_function" # Placeholder
      authorization   = "COGNITO_USER_POOLS"
      authorizer      = true
      enable_cors     = true
    }
  ]
}

locals {
  lambda_function_map = {
    "updatelanguage_function"   = data.aws_lambda_function.update_language.invoke_arn
    "getuser_function"          = data.aws_lambda_function.get_user.invoke_arn
    "getuserlanguages_function" = data.aws_lambda_function.get_user_languages.invoke_arn
    "removeuserlanguage_function" = data.aws_lambda_function.remove_user_language.invoke_arn
    "presignup_function"        = data.aws_lambda_function.pre_sign_up.invoke_arn
  }

  lambda_arn_map = {
    "updatelanguage_function"   = data.aws_lambda_function.update_language.arn
    "getuser_function"          = data.aws_lambda_function.get_user.arn
    "getuserlanguages_function" = data.aws_lambda_function.get_user_languages.arn
    "removeuserlanguage_function" = data.aws_lambda_function.remove_user_language.arn
    "presignup_function"        = data.aws_lambda_function.pre_sign_up.arn
  }
}