resource "aws_api_gateway_rest_api" "rest_api" {
  name = "polybara-api"
}

# Define the specific resource for the /updatelanguage path
resource "aws_api_gateway_resource" "update_language" {
  rest_api_id = aws_api_gateway_rest_api.rest_api.id
  parent_id   = aws_api_gateway_rest_api.rest_api.root_resource_id
  path_part   = "updatelanguage"
}

resource "aws_api_gateway_authorizer" "authorizer" {
  name        = "CognitoUserPoolAuthorizer"
  type        = "COGNITO_USER_POOLS"
  rest_api_id = aws_api_gateway_rest_api.rest_api.id
  provider_arns = [aws_cognito_user_pool.user_pool.arn]
}

# Create the method for the /updatelanguage resource
resource "aws_api_gateway_method" "update_language" {
  rest_api_id   = aws_api_gateway_rest_api.rest_api.id
  resource_id   = aws_api_gateway_resource.update_language.id
  http_method   = "POST"
  authorization = "COGNITO_USER_POOLS"
  authorizer_id = aws_api_gateway_authorizer.authorizer.id

  request_parameters = {
    "method.request.header.Authorization" = true
  }
}

# Integrate the /updatelanguage method with the Lambda function
resource "aws_api_gateway_integration" "update_language_lambda" {
  rest_api_id = aws_api_gateway_rest_api.rest_api.id
  resource_id = aws_api_gateway_resource.update_language.id
  http_method = aws_api_gateway_method.update_language.http_method

  integration_http_method = "POST"
  type                    = "AWS_PROXY"
  uri                     = data.aws_lambda_function.update_language.invoke_arn
}

resource "aws_api_gateway_method" "update_language_options" {
  rest_api_id   = aws_api_gateway_rest_api.rest_api.id
  resource_id   = aws_api_gateway_resource.update_language.id
  http_method   = "OPTIONS"
  authorization = "NONE"
}

resource "aws_api_gateway_integration" "update_language_options_integration" {
  rest_api_id = aws_api_gateway_rest_api.rest_api.id
  resource_id = aws_api_gateway_resource.update_language.id
  http_method = aws_api_gateway_method.update_language_options.http_method

  type = "MOCK"

  request_templates = {
    "application/json" = "{\"statusCode\": 200}"
  }

  passthrough_behavior = "NEVER"
}

resource "aws_api_gateway_integration_response" "options_integration_response" {
  rest_api_id = aws_api_gateway_rest_api.rest_api.id
  resource_id = aws_api_gateway_resource.update_language.id
  http_method = aws_api_gateway_method.update_language_options.http_method
  status_code = "200"
  
  response_parameters = {
    "method.response.header.Access-Control-Allow-Headers" = "'Content-Type,X-Amz-Date,Authorization,X-Api-Key,X-Amz-Security-Token'",
    "method.response.header.Access-Control-Allow-Methods" = "'*'",
    "method.response.header.Access-Control-Allow-Origin"  = "'*'"
  }

  depends_on = [
    aws_api_gateway_method.update_language_options,
    aws_api_gateway_integration.update_language_options_integration
  ]
}

resource "aws_api_gateway_method_response" "update_language_options_response" {
  rest_api_id = aws_api_gateway_rest_api.rest_api.id
  resource_id = aws_api_gateway_resource.update_language.id
  http_method = aws_api_gateway_method.update_language_options.http_method
  status_code = "200"

  response_parameters = {
    "method.response.header.Access-Control-Allow-Origin"  = true
    "method.response.header.Access-Control-Allow-Methods" = true
    "method.response.header.Access-Control-Allow-Headers" = true
  }
}

# Deploy the API to a stage
resource "aws_api_gateway_deployment" "deployment" {
  rest_api_id = aws_api_gateway_rest_api.rest_api.id
  stage_name  = "prod"

  depends_on = [
    aws_api_gateway_integration.update_language_lambda,
    aws_api_gateway_method.update_language,
    aws_api_gateway_method.update_language_options,
    aws_api_gateway_integration.update_language_options_integration,
    aws_api_gateway_integration_response.options_integration_response,
    aws_api_gateway_method_response.update_language_options_response
  ]
}

resource "aws_lambda_permission" "api_gateway_invoke" {
  statement_id  = "AllowAPIGatewayInvoke"
  action        = "lambda:InvokeFunction"
  function_name = data.aws_lambda_function.update_language.function_name
  principal     = "apigateway.amazonaws.com"
  source_arn    = "${aws_api_gateway_rest_api.rest_api.execution_arn}/*/*"
}