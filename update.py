#!/usr/bin/env python3
#-*- coding: utf-8 -*-

import os, subprocess
import argparse

def log(*args):
    print(*args)

def run(cmd, exitIfError=False, print=True):
    result = subprocess.run(cmd, capture_output=True, shell=True)
    if print:
        log(result.stdout.decode('utf-8'))
        log(result.stderr.decode('utf-8'))
    if exitIfError and result.returncode != 0:
        if type(cmd) == type([]):
            cmd = " ".join(cmd)
        log(f"`{cmd}` exited with {result.returncode}." )
        exit(result.returncode)
    return result

parser = argparse.ArgumentParser(description="Update Watchtower crawler and server.", add_help=True)
parser.add_argument("-n", action='store_true', default=False, help="Do not pull from git, only compile.")

args = parser.parse_args()

if not args.n:
    run(["git pull"], exitIfError=True)

# Compile crawler
run("rm -r ./bin/crawler/*")
run("""dotnet build -o ./bin/crawler --no-self-contained ./Watchtower.Crawler/""", exitIfError=True)
run("ln -s `pwd`/bin/crawler/Watchtower.Crawler ./bin/Watchtower.Crawler", exitIfError=False)

# TODO: Compile server

run("date >> ./bin/last_success.txt")
