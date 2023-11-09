FROM openpolicyagent/opa
EXPOSE 8181
WORKDIR /app
COPY . ./
ENTRYPOINT ["/opa"]
CMD ["run","--server", "./policy.rego"]
