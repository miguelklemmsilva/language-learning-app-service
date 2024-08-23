provider "aws" {
  region = "eu-west-2"
}

terraform {
  backend "s3" {
    bucket = "miguelklemmsilva-tf-state-bucket"
    key    = "terraform.tfstate"
    region = "eu-west-2"
  }
}