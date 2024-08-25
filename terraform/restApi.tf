resource "aws_api_gateway_rest_api" "rest_api" {
  name = "polybara-api"
}

resource "aws_api_gateway_authorizer" "authorizer" {
  name        = "CognitoUserPoolAuthorizer"
  type        = "COGNITO_USER_POOLS"
  rest_api_id = aws_api_gateway_rest_api.rest_api.id
  provider_arns = [aws_cognito_user_pool.user_pool.arn]
}

# Create API Gateway resources dynamically
resource "aws_api_gateway_resource" "routes" {
  for_each    = { for route in var.api_routes : route.path => route }
  rest_api_id = aws_api_gateway_rest_api.rest_api.id
  parent_id   = aws_api_gateway_rest_api.rest_api.root_resource_id
  path_part   = each.value.path
}

# Create methods and integrate with Lambda functions
resource "aws_api_gateway_method" "routes" {
  for_each       = { for route in var.api_routes : route.path => route }
  rest_api_id    = aws_api_gateway_rest_api.rest_api.id
  resource_id    = aws_api_gateway_resource.routes[each.value.path].id
  http_method    = each.value.http_method
  authorization  = each.value.authorization

  authorizer_id = each.value.authorizer ? aws_api_gateway_authorizer.authorizer.id : null

  request_parameters = each.value.authorizer ? {
    "method.request.header.Authorization" = true
  } : {}
}

resource "aws_api_gateway_integration" "routes" {
  for_each                = { for route in var.api_routes : route.path => route }
  rest_api_id             = aws_api_gateway_rest_api.rest_api.id
  resource_id             = aws_api_gateway_resource.routes[each.value.path].id
  http_method             = aws_api_gateway_method.routes[each.value.path].http_method
  integration_http_method = "POST"
  type                    = "AWS_PROXY"
  uri                     = local.lambda_function_map[each.value.lambda_function]
}

resource "aws_lambda_permission" "api_gateway_invoke" {
  for_each      = { for route in var.api_routes : route.path => route }
  statement_id  = "AllowAPIGatewayInvoke${each.value.path}"
  action        = "lambda:InvokeFunction"
  function_name = local.lambda_arn_map[each.value.lambda_function]
  principal     = "apigateway.amazonaws.com"
  source_arn    = "${aws_api_gateway_rest_api.rest_api.execution_arn}/*/*"
}

# Create OPTIONS method and CORS integration for each route if enabled
resource "aws_api_gateway_method" "cors_options" {
  for_each       = { for route in var.api_routes : route.path => route if route.enable_cors }
  rest_api_id    = aws_api_gateway_rest_api.rest_api.id
  resource_id    = aws_api_gateway_resource.routes[each.value.path].id
  http_method    = "OPTIONS"
  authorization  = "NONE"
}

resource "aws_api_gateway_integration" "cors_integration" {
  for_each    = { for route in var.api_routes : route.path => route if route.enable_cors }
  rest_api_id = aws_api_gateway_rest_api.rest_api.id
  resource_id = aws_api_gateway_resource.routes[each.value.path].id
  http_method = aws_api_gateway_method.cors_options[each.key].http_method

  type = "MOCK"

  request_templates = {
    "application/json" = "{\"statusCode\": 200}"
  }

  passthrough_behavior = "NEVER"
}

resource "aws_api_gateway_integration_response" "cors_integration_response" {
  for_each    = { for route in var.api_routes : route.path => route if route.enable_cors }
  rest_api_id = aws_api_gateway_rest_api.rest_api.id
  resource_id = aws_api_gateway_resource.routes[each.value.path].id
  http_method = aws_api_gateway_method.cors_options[each.key].http_method
  status_code = "200"

  response_parameters = {
    "method.response.header.Access-Control-Allow-Headers" = "'Content-Type,X-Amz-Date,Authorization,X-Api-Key,X-Amz-Security-Token'",
    "method.response.header.Access-Control-Allow-Methods" = "'*'",
    "method.response.header.Access-Control-Allow-Origin"  = "'*'"
  }
}

resource "aws_api_gateway_method_response" "cors_method_response" {
  for_each    = { for route in var.api_routes : route.path => route if route.enable_cors }
  rest_api_id = aws_api_gateway_rest_api.rest_api.id
  resource_id = aws_api_gateway_resource.routes[each.value.path].id
  http_method = aws_api_gateway_method.cors_options[each.key].http_method
  status_code = "200"

  response_parameters = {
    "method.response.header.Access-Control-Allow-Headers" = true
    "method.response.header.Access-Control-Allow-Methods" = true
    "method.response.header.Access-Control-Allow-Origin"  = true
  }
}