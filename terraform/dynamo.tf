resource "aws_dynamodb_table" "users" {
  name         = "users"
  billing_mode = "PAY_PER_REQUEST"
  hash_key     = "UserId"
  
  attribute {
    name = "UserId"
    type = "S"
  }
}

resource "aws_dynamodb_table" "user_languages" {
  name        = "user_languages"
  billing_mode = "PAY_PER_REQUEST"
  hash_key     = "UserId"
  range_key    = "Language"
  
  attribute {
    name = "UserId"
    type = "S"
  }
  
  attribute {
    name = "Language"
    type = "S"
  }
}

resource "aws_dynamodb_table" "vocabulary" {
    name         = "vocabulary"
    billing_mode = "PAY_PER_REQUEST"
    hash_key     = "UserId"
    range_key    = "sk"
    
    attribute {
        name = "UserId" 
        type = "S"
    }
    
    attribute {
        name = "sk"
        type = "S"
    }
}