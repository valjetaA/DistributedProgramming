cd ../Valuator/
dotnet run --urls "http://localhost:5001/" &
dotnet run --urls "http://localhost:5002/" &

nginx -c /opt/homebrew/etc/nginx/nginx.conf

