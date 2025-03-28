provider "aws" {
  region = var.AWS_REGION
}

terraform {
  backend "s3" {
    bucket = "miguelklemmsilva-tf-state-bucket"
    key    = "terraform.tfstate"
    region = "eu-west-2"
  }
}