provider "aws" {
  alias  = "us_east_1"
  region = "us-east-1"
}

# must be imported from us-east-1 for cloudfront
data "aws_acm_certificate" "domain_certificate" {
  domain = "api.miguelklemmsilva.com"
  statuses = ["ISSUED"]
  provider    = aws.us_east_1
  most_recent = true
}

resource "aws_cloudfront_response_headers_policy" "cors_policy" {
  name = "CORSResponseHeadersPolicy"

  cors_config {
    access_control_allow_credentials = false
    access_control_allow_headers {
      items = ["Content-Type", "Authorization", "X-Amz-Date", "X-Api-Key", "X-Amz-Security-Token"]
    }
    access_control_allow_methods {
      items = ["HEAD", "DELETE", "GET", "POST", "PUT", "OPTIONS", "PATCH"]
    }
    access_control_allow_origins {
      items = ["*"]
    }
    origin_override = true
  }
}

resource "aws_cloudfront_distribution" "api_distribution" {
  aliases = ["api.miguelklemmsilva.com"]
  origin {
    domain_name = "${aws_api_gateway_deployment.deployment.rest_api_id}.execute-api.${var.AWS_REGION}"
    origin_id   = "api-gateway-origin"

    custom_origin_config {
      http_port              = 80
      https_port             = 443
      origin_protocol_policy = "https-only"
      origin_ssl_protocols = ["TLSv1.2"]
    }
  }

  default_cache_behavior {
    target_origin_id       = "api-gateway-origin"
    allowed_methods = ["HEAD", "GET", "POST", "PUT", "DELETE", "OPTIONS", "PATCH"]
    cached_methods = ["HEAD", "GET", "OPTIONS"]
    viewer_protocol_policy = "redirect-to-https"

    forwarded_values {
      query_string = true
      headers = ["Authorization"]
      cookies {
        forward = "none"
      }
    }
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