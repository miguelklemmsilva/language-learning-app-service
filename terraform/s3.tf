terraform {
  required_providers {
    aws = {
      source  = "hashicorp/aws"
      version = "5.81.0"
    }
  }
}
data "aws_s3_object" "lambda_artifact_object" {
  bucket        = "polybara-artifacts"
  key           = "Lambdas.zip"
  checksum_mode = "ENABLED"
}

resource "aws_s3_bucket" "logging_bucket" {
  bucket = "polybara-cloudfront-logging-bucket"
}

resource "aws_s3_bucket_ownership_controls" "logging_ownership_controls" {
  bucket = aws_s3_bucket.logging_bucket.id

  rule {
    object_ownership = "BucketOwnerPreferred"
  }
}