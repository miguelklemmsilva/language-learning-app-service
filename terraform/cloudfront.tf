provider "aws" {
  alias  = "us_east_1"
  region = "us-east-1"
}

# must be imported from us-east-1 for cloudfront
data "aws_acm_certificate" "domain_certificate" {
  domain      = "api.miguelklemmsilva.com"
  statuses = ["ISSUED"]
  provider    = aws.us_east_1
  most_recent = true
}

data "aws_cloudfront_cache_policy" "cache_policy" {
  name = "Managed-CachingDisabled"
}

data "aws_cloudfront_origin_request_policy" "origin_request_policy" {
  name = "Managed-AllViewerExceptHostHeader"
}

data "aws_cloudfront_response_headers_policy" "response_headers_policy" {
  name = "Managed-CORS-With-Preflight"
}

resource "aws_cloudfront_distribution" "api_distribution" {
  aliases = ["api.miguelklemmsilva.com"]
  origin {
    domain_name = "${aws_api_gateway_deployment.deployment.rest_api_id}.execute-api.${var.AWS_REGION}.amazonaws.com"
    origin_path = "/prod"
    origin_id   = "api-gateway-origin"

    custom_origin_config {
      http_port              = 80
      https_port             = 443
      origin_protocol_policy = "https-only"
      origin_ssl_protocols = ["TLSv1.2"]
    }
  }

  default_cache_behavior {
    target_origin_id         = "api-gateway-origin"
    viewer_protocol_policy   = "redirect-to-https"
    cache_policy_id          = data.aws_cloudfront_cache_policy.cache_policy.id
    origin_request_policy_id = data.aws_cloudfront_origin_request_policy.origin_request_policy.id
    allowed_methods = ["HEAD", "GET", "POST", "PUT", "DELETE", "OPTIONS", "PATCH"]
    cached_methods = ["HEAD", "GET", "OPTIONS"]

    response_headers_policy_id = data.aws_cloudfront_response_headers_policy.response_headers_policy.id
  }

  enabled         = true
  is_ipv6_enabled = true
  comment         = "Distribution in front of API Gateway for CORS handling"
  price_class     = "PriceClass_All"

  logging_config {
    bucket = aws_s3_bucket.logging_bucket.bucket_domain_name
  }

  restrictions {
    geo_restriction {
      restriction_type = "none"
    }
  }

  viewer_certificate {
    acm_certificate_arn = data.aws_acm_certificate.domain_certificate.arn
    ssl_support_method  = "sni-only"
  }
}