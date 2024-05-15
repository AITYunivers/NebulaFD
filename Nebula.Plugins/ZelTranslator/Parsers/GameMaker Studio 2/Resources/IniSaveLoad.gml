function IniSaveLoad() {
	
}

function ini_close_with_kf(fileName, callWhenDone) {
    var _inistring = ini_close();
    with (objKotfile) {
        queueFile(fileName, true /* saving */, callWhenDone, makeBuff(_inistring));
    }
}

function ini_open_with_kf(fileName, callWhenDone) {
    with (objKotfile) {
        queueFile(fileName, false /* loading */, method({whenDone:callWhenDone}, function (args) {
            var stringContents = buffer_read(args.bufferIndex, buffer_string);
            ini_open_from_string(stringContents);
			global.ini_string = stringContents
            self.whenDone();
        }));
    }
}