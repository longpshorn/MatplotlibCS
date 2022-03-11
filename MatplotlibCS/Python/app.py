# -*- coding: utf-8 -*-
import json
import sys
from matplotlib import rc
from flask import Flask, request, json
from matplotlib_cs import MatplotlibCS
import logging

# Script builds a matplotlib figure based on information, passed to it through json file.
# Path to the JSON must be passed in first command line argument.

# a trick to enable text labels in cyrillic
rc('font', **{'sans-serif': 'Arial','family': 'sans-serif'})

app = Flask(__name__)

def main(args):
    host = "127.0.0.1"
    app.run(host=host, port=57123, debug=True)

@app.route('/', methods=['POST', 'GET'])
def api_check_alive():
    return '', 200

@app.route('/plot', methods=['POST', 'GET'])
def api_plot():
    json_raw = request.json
    app.logger.log(logging.INFO, json_raw)
    task = json.loads(json_raw)
    mpl = MatplotlibCS(task)
    mpl.build_figure()

    return '', 200

@app.route('/kill', methods=['POST', 'GET'])
def api_kill():
    raise RuntimeError('Stop web service')

# entry point
if __name__ == "__main__":
    main(sys.argv[1:])
