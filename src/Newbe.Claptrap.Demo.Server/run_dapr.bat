dapr run -a newbe_claptrap_demo_server -p 5000 -H 50001 -G 50002 --components-path ./dapr/components --config ./dapr/config/config.yml -- dotnet run --launch-profile "Newbe.Claptrap.Demo.Server"
