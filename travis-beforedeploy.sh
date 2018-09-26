#!/usr/bin/env bash
cd ${TRAVIS_BUILD_DIR}/CLI/bin/release/netcoreapp2.0/win10-x64/publish
zip -r ${TRAVIS_BUILD_DIR}/clamper-cli_win-x64-${TRAVIS_TAG}.zip ./*
cd ${TRAVIS_BUILD_DIR}/CLI/bin/release/netcoreapp2.0/ubuntu.16.10-x64/publish
zip -r ${TRAVIS_BUILD_DIR}/clamper-cli_linux-x64-${TRAVIS_TAG}.zip ./*
cd ${TRAVIS_BUILD_DIR}/CLI/bin/release/netcoreapp2.0/osx-x64/publish
zip -r ${TRAVIS_BUILD_DIR}/clamper-cli_osx-x64-${TRAVIS_TAG}.zip ./*
cd  ${TRAVIS_BUILD_DIR}
