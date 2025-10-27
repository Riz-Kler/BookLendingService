output "alb_dns_name" {
  description = "Load Balancer public DNS name"
  value       = aws_lb.api.dns_name
}

# (Optional – handy during deploys)
output "ecs_cluster_name" {
  value = aws_ecs_cluster.this.name
}

output "ecs_service_name" {
  value = aws_ecs_service.api.name
}