﻿using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace App {
    public class MirrorService {

        private DateTime _lastChange;
        private string _mirrorLabel = "";
        private string _currentLabel = "";
        private MirrorClient _client;
        private static object _sync = new object();


        public MirrorService(string mirrorUrl) {
            _client = new MirrorClient(mirrorUrl);
        }

        public void Start() {
            Task.Factory.StartNew(async () => {
                while (true) {
                    await Task.Delay(TimeSpan.FromSeconds(2));

                    var label = _currentLabel;
                    if (DateTime.Now - _lastChange < TimeSpan.FromSeconds(1) || _mirrorLabel == label) {
                        continue;
                    }
                    if (await _client.ChangeUser(label)) {
                        Debug.WriteLine("SmartMirror label set to " + label);
                        _mirrorLabel = label;
                    }
                    else {
                        Debug.WriteLine("Could not change SmartMirror label!");
                    }
                }
            }, TaskCreationOptions.LongRunning);

        }

        public void SetNewLabel(string label) {
            if (label == _currentLabel) {
                return;
            }
            _currentLabel = label;
            _lastChange = DateTime.Now;
        }
    }
}