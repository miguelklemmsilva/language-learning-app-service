provider "aws" {
  region = "eu-west-2"
}

terraform {
  backend "s3" {
    bucket = "miguelklemmsilva-lla-tf-state-bucket"
    key    = "terraform.tfstate"
    region = "eu-west-2"
  }
}

resource "aws_cognito_user_pool" "user-pool" {
  name = "poly-bara"

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

