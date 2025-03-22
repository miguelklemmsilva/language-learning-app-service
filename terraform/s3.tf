data "aws_s3_object" "lambda_artifact_object" {
  bucket = "polybara-artifacts"
  key = "Lambdas.zip"
}
