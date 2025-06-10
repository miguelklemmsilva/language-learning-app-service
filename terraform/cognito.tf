resource "aws_cognito_user_pool" "user_pool" {
  name = "poly-bara"

  # Specify the email as a required standard attribute
  auto_verified_attributes = ["email"]

  # Specify that the email is required and should be used as a username
  username_attributes = ["email"]

  # Enable email verification for sign-up
  verification_message_template {
    default_email_option = "CONFIRM_WITH_CODE"  # Options: CONFIRM_WITH_LINK or CONFIRM_WITH_CODE
  }

  account_recovery_setting {
    recovery_mechanism {
      name     = "verified_email"
      priority = 1
    }

    recovery_mechanism {
      name     = "verified_phone_number"
      priority = 2
    }
  }

  lambda_config {
    post_confirmation = aws_lambda_function.pre_sign_up.arn
  }
}

resource "aws_cognito_user_pool_client" "userpool_client" {
  name                                 = "client"
  user_pool_id                         = aws_cognito_user_pool.user_pool.id
  
  # OAuth Configuration
  callback_urls                        = ["https://miguelklemmsilva.com/dashboard", "http://localhost:5173/dashboard", "https://miguelklemmsilva.com/home", "polybara://home"]
  logout_urls                          = ["https://miguelklemmsilva.com", "http://localhost:5173", "polybara://"]
  allowed_oauth_flows_user_pool_client = true
  allowed_oauth_flows                  = ["code"] # Remove "implicit" as it's less secure
  allowed_oauth_scopes                 = ["email", "openid", "profile"]
  supported_identity_providers         = ["COGNITO", "Google"]
  
  id_token_validity                   = 60       # 1 hour (in minutes)
  access_token_validity               = 60       # 1 hour (in minutes)
  refresh_token_validity              = 43200    # 30 days (in minutes)
  token_validity_units {
    id_token      = "minutes"
    access_token  = "minutes"
    refresh_token = "minutes"
  }

  # Authentication Flows
  explicit_auth_flows = [
    "ALLOW_USER_PASSWORD_AUTH",
    "ALLOW_REFRESH_TOKEN_AUTH"
  ]

  # Prevent common errors
  prevent_user_existence_errors = "ENABLED"
  enable_token_revocation      = true
}

resource "aws_cognito_user_pool_domain" "main" {
  domain       = "poly-bara"
  user_pool_id = aws_cognito_user_pool.user_pool.id
}

resource "aws_cognito_identity_pool" "identity_pool" {
  identity_pool_name               = "identity pool"
  allow_unauthenticated_identities = false
  cognito_identity_providers {
    client_id               = aws_cognito_user_pool_client.userpool_client.id
    provider_name           = aws_cognito_user_pool.user_pool.endpoint
    server_side_token_check = false
  }
}

data "aws_ssm_parameter" "google_client_id" {
  name = "/myapp/google/client_id"
}

data "aws_ssm_parameter" "google_client_secret" {
  name = "/myapp/google/client_secret"
}

resource "aws_cognito_identity_provider" "google" {
  user_pool_id  = aws_cognito_user_pool.user_pool.id
  provider_name = "Google"
  provider_type = "Google"

  provider_details = {
    authorize_scopes              = "email openid"
    client_id                     = data.aws_ssm_parameter.google_client_id.value
    client_secret                 = data.aws_ssm_parameter.google_client_secret.value
    token_url                     = "https://www.googleapis.com/oauth2/v4/token"
    token_request_method          = "POST"
    oidc_issuer                   = "https://accounts.google.com"
    authorize_url                 = "https://accounts.google.com/o/oauth2/v2/auth"
    attributes_url                = "https://people.googleapis.com/v1/people/me?personFields="
    attributes_url_add_attributes = "true"
  }

  attribute_mapping = {
    email    = "email"
    username = "sub"
  }
}
