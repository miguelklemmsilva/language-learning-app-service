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
}

resource "aws_cognito_user_pool_client" "userpool_client" {
  name                                 = "client"
  user_pool_id                         = aws_cognito_user_pool.user_pool.id
  callback_urls                        = ["https://miguelklemmsilva.com/"]
  allowed_oauth_flows_user_pool_client = true
  allowed_oauth_flows                  = ["code", "implicit"]
  allowed_oauth_scopes                 = ["email", "openid"]
  supported_identity_providers         = ["COGNITO"]
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