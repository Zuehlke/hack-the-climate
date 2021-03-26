#!/bin/bash

# pip3 install pdfminer
# apt-get install parallel

for filename in ./pdf-download/*.pdf; do
	newfile="./text/`basename $filename`"
	echo "pdf2txt.py -o $newfile -t text $filename 2> /dev/null" >> tasks.txt
done

parallel < tasks.txt



