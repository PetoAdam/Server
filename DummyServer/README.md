# The official CovidShooter server

## Building with Docker
If you want to build the server for yourself navigate to the `DummyServer` folder and run `docker build --network=host -t covidshooter .`
## Running with docker
`docker run -p 26950:26950 -v /PATH/TO/database2.txt:/app/database2.txt -d --rm -it --name covidshooter covidshooter`
