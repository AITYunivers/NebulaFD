/// @description file_text_write_* but fluent and with buffers. For use with Kotfile.
/// @param {Mixed} [argInitialContentsStringOpt] Initial contents or undefined.
function TextFileWrite(argInitialContentsStringOpt) constructor {
	// configuration:
	__tfwln = "\r\n"; // might want to change this to "\n" if you're on Linux, but GM always uses \r\n
	__tfwdecplaces = 17; // how many digits after the dot, 17 is a sane default.
	
	// the contents:
	__tfwstring = (is_undefined(argInitialContentsStringOpt) ? "" : string(argInitialContentsStringOpt));
	
	// the methods:
	writeString = function(argWhatAny) {
		__tfwstring += string(argWhatAny);
		return self;
	};
	
	writeReal = function(argWhatNumber) {
		__tfwstring += string_format(argWhatNumber, 1, __tfwdecplaces);
		return self;
	};
	
	writeLn = function() {
		__tfwstring += __tfwln;
		return self;
	};
	
	toBuffer = function(argBufferOutOpt) {
		var __tfwbuff = buffer_create(string_byte_length(__tfwstring), buffer_grow, 1);
		buffer_write(__tfwbuff, buffer_text, __tfwstring);
		// the seek position will be after the text...
		// since you might want to append something...
		if (!is_undefined(argBufferOutOpt)) {
			// return buffer index into a struct, and return self
			// example:
			/*
				var buf = { val: -1 }; // dummy struct, will contain the buf.
				new TextFileWrite()
					.writeString("Hello!")
					.writeLn()
					.writeString("This is a number: ")
					.writeReal(1337.228420)
					.writeLn()
					.toBuffer(buf);
					// might do something else here?
				// should have the actual buffer index now:
				var bufind = buf.val;
				kf.
					queueFile(
						"test.sav",
						true,
						onSave,
						bufind
					);
				
				// do you get the idea...?
			*/
			argBufferOutOpt.val = __tfwbuff;
			return self;
		}
		else {
			// just return the buffer index.
			// example:
			/*
				var buf = new TextFileWrite()
					.writeString("Hello!")
					.writeLn()
					.toBuffer();
				// and now:
				kf.
					queueFile(
						"test.sav",
						true,
						onSave,
						buf
					);
				
				// do you get the idea...?
			*/
			return __tfwbuff;
		}
	};
	
	toString = function() {
		// NOT RECOMMENDED.
		return __tfwstring;	
	};
}
