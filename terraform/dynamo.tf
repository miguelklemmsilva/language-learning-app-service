resource "aws_dynamodb_table" "users" {
  name         = "users"
  billing_mode = "PAY_PER_REQUEST"
  hash_key     = "UserId"
  
  attribute {
    name = "UserId"
    type = "S"
  }
}