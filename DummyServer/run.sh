#!/bin/bash

docker run  --rm \
            -it \
            -p 0.0.0.0:26950:26950/tcp \
            -p 0.0.0.0:26950:26950/udp \
            mmserver