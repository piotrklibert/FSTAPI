#! env bash

fsc -a utils.fs
fsc -r utils.dll -a params.fs
fsc -r utils.dll -r params.dll -a client.fs
fsc -r utils.dll -r params.dll -r client.dll test.fs
