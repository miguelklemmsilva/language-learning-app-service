resource "aws_api_gateway_rest_api" "rest_api" {
  name = "polybara-api"
}

resource "aws_api_gateway_authorizer" "authorizer" {
  name        = "CognitoUserPoolAuthorizer"
  type        = "COGNITO_USER_POOLS"
  rest_api_id = aws_api_gateway_rest_api.rest_api.id
  provider_arns = [aws_cognito_user_pool.user_pool.arn]
}

resource "aws_api_gateway_resource" "routes" {
  for_each = toset([for route in var.api_routes : route.path])
  rest_api_id = aws_api_gateway_rest_api.rest_api.id
  parent_id   = aws_api_gateway_rest_api.rest_api.root_resource_id
  path_part   = each.key
}

resource "aws_api_gateway_method" "routes" {
  for_each      = {for idx, route in var.api_routes : "${route.path}-${route.http_method}" => route}
  rest_api_id   = aws_api_gateway_rest_api.rest_api.id
  resource_id   = aws_api_gateway_resource.routes[each.value.path].id
  http_method   = each.value.http_method
  authorization = "COGNITO_USER_POOLS"
  authorizer_id = aws_api_gateway_authorizer.authorizer.id

  request_parameters = {
    "method.request.header.Authorization" = true
  }
}

resource "aws_api_gateway_integration" "routes" {
  for_each                = {for idx, route in var.api_routes : "${route.path}-${route.http_method}" => route}
  rest_api_id             = aws_api_gateway_rest_api.rest_api.id
  resource_id             = aws_api_gateway_resource.routes[each.value.path].id
  http_method             = each.value.http_method
  integration_http_method = "POST"
  type                    = "AWS_PROXY"
  uri                     = aws_lambda_function.api_route_functions[each.value.lambda_function].invoke_arn

  depends_on = [aws_api_gateway_method.routes]
}

resource "aws_lambda_permission" "api_gateway_invoke" {
  for_each      = {for idx, route in var.api_routes : "${route.path}-${route.http_method}" => route}
  statement_id  = "AllowAPIGatewayInvoke${replace(each.value.path, "/", "")}${each.value.http_method}"
  action        = "lambda:InvokeFunction"
  function_name = aws_lambda_function.api_route_functions[each.value.lambda_function].function_name
  principal     = "apigateway.amazonaws.com"
  source_arn    = "${aws_api_gateway_rest_api.rest_api.execution_arn}/*/*"
}

resource "aws_api_gateway_deployment" "deployment" {
  rest_api_id = aws_api_gateway_rest_api.rest_api.id

  triggers = {
    redeployment = sha1(jsonencode([
      aws_api_gateway_resource.routes,
      aws_api_gateway_method.routes,
      aws_api_gateway_integration.routes,
    ]))
  }

  lifecycle {
    create_before_destroy = true
  }
}

resource "aws_api_gateway_stage" "stage" {
  rest_api_id   = aws_api_gateway_rest_api.rest_api.id
  stage_name    = "prod"
  deployment_id = aws_api_gateway_deployment.deployment.id
}