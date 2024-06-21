#!/bin/sh
docker build . -t opuscross
# clean up old zlib artifacts
rm -rf linux osx win
# produce zlib artifacts in container
OPUSCROSS_CONTAINER=$(docker create opuscross --name opuscross)
# extract zlib artifacts
docker cp "${OPUSCROSS_CONTAINER}:/app/" ./
# clean up container
docker rm "${OPUSCROSS_CONTAINER}"
mv app/* ./
rm -rf app
# consider purging your images