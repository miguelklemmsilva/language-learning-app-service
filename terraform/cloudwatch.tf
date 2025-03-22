resource "aws_iam_role_policy" "lambda_logging_policy" {
  for_each = { for route in var.api_routes : route.lambda_function => route }

  name = "${each.key}-logging-policy"
  role = aws_iam_role.lambda_role[each.key].id

  policy = jsonencode({
    Version = "2012-10-17",
    Statement = [
      {
        Effect = "Allow",
        Action = [
          "logs:CreateLogGroup",
          "logs:CreateLogStream",
          "logs:PutLogEvents"
        ],
        Resource = "arn:aws:logs:*:*:*"
      }
    ]
  })
}

resource "aws_iam_role_policy" "pre_sign_up_logging_policy" {
  name = "PreSignUp-logging-policy"
  role = aws_iam_role.pre_sign_up_role.id

  policy = jsonencode({
    Version = "2012-10-17",
    Statement = [
      {
        Effect = "Allow",
        Action = [
          "logs:CreateLogGroup",
          "logs:CreateLogStream",
          "logs:PutLogEvents"
        ],
        Resource = "arn:aws:logs:*:*:*"
      }
    ]
  })
}

resource "aws_cloudwatch_log_group" "api_route_log_groups" {
  for_each = aws_lambda_function.api_route_functions

  name              = "/aws/lambda/${each.value.function_name}"
  retention_in_days = 1
}

resource "aws_cloudwatch_log_group" "pre_sign_up_log_group" {
  name              = "/aws/lambda/${aws_lambda_function.pre_sign_up.function_name}"
  retention_in_days = 1
}