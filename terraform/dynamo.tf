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
  name         = "user_languages"
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

resource "aws_dynamodb_table" "sentences" {
  name         = "sentences"
  billing_mode = "PAY_PER_REQUEST"
  hash_key     = "id"

  attribute {
    name = "id"
    type = "S"
  }

  attribute {
    name = "srcLang"
    type = "S"
  }

  attribute {
    name = "tgtLang"
    type = "S"
  }

  attribute {
    name = "tokenCount"
    type = "N"
  }

  global_secondary_index {
    name            = "LangTokenIndex"
    hash_key        = "srcLang"
    range_key       = "tokenCount"
    projection_type = "ALL"
  }
}