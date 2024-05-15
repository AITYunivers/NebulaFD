/// @description Kotfile: This is here to prevent a Kotfile() call from reinitializing anything....
function Kotfile() {
	/* oh and to silence a bug in Feather ;-;
	*/
	return "Why are you doing this when you are the architect of your own vision..?";
}

/// @description Kotfile: Must be called once in some persistent object to initialize Kotfile.
function KotfileInit() {
	/* KF CONFIGURATION START */
	/* please edit this to your liking! you cannot change those at runtime legally (and why would you?) */
	enableTrace         = true;            // bool:   enable debug trace or not? useful if you're dev-ing games for PS.
	allowImplicitGroups = true;            // bool:   if an async group wasn't started, start and end one implicitly.
	tracePrefix         = "[KfTrace]: ";   // string: debug trace prefix
	errorPrefix         = "[KfError]: ";   // string: exception messages prefix
	/* these settings can be changed with setGroupOption() as well: */
	groupName           = "KotfileDefGrp"; // string: name of the async group
	slotTitle           = "KotfileSavSlt"; // string: slot title
	subTitle            = "KotfileSubTlt"; // string: slot subtitle
	showDialog          = false;           // bool:   do show save selection dialog?
	psCreateBackup      = false;           // bool:   (os_ps5 os_ps4 only), ask the system to do a backup?
	savePadIndex        = -1;              // number: to which pad to save, usually 0 but plz change at runtime. -1 means don't set.
	accountIndex        = -1;              // number: NO idea what it's supposed to do??? -1 means don't set.
	/* KF CONFIGURATION END */
	
	/* KF PRIVATE MEMBERS START */
	/* all private methods and members are prefixed with __kf */
	__kfVersion = "1.0.4";
	__kfAuthor = "nkrapivindev";
	__kfDate = "07.02.2022 0:40 (DD.MM.YYYY, HH:mm 24hr, UTC+5 Asia/Yekaterinburg)";
	__kfGroups = [];
	__kfCurrentGroup = undefined;
	__kfIsSwitch = os_type == os_switch;
	__kfIsPSOrbEro = os_type == os_ps4 || os_type == os_ps5; // ORBIS and Prospero! They're very similar.
	/* KF PRIVATE MEMBERS END */
	
	/* KF PRIVATE METHODS START */
	__kfDateTimer = function() {
		// returns current datetime as a string.
		return date_datetime_string(date_current_datetime());	
	};
	
	__kfOutputter = function(argMsgString) {
		// you're free to replace this if you REALLY have to:
		show_debug_message(argMsgString);	
	};
	
	__kfThrower = function(argMsgString) {
		throw string(argMsgString);	
	};
	
	__kfFormat = function(argMsgString, argArgsArray) {
		var str = string(argMsgString);
		if (!is_undefined(argArgsArray)) {
			for (var i = 0; i < array_length(argArgsArray); ++i) {
				str = string_replace(str, "{}", string(argArgsArray[@ i]));
			}
		}
		
		return str;
	};
	
	__kfTrace = function(argMsgString, argArgsArrayOpt) {
		if (!enableTrace) return false;
		var s = __kfFormat(argMsgString, argArgsArrayOpt);
		s = string_insert(string(tracePrefix), s, 1);
		__kfOutputter(s);
		return true;
	};
	
	__kfThrow = function(argMsgString, argArgsArrayOpt) {
		var s = __kfFormat(argMsgString, argArgsArrayOpt);
		s = string_insert(string(errorPrefix), s, 1);
		__kfThrower(s);
		return s;
	};
	
	__kfDispatchGroup = function() {
		if (groups() < 1) {
			__kfTrace("There are no async groups left for dispatch.");
			return -1;	
		}
		
		var kfgrp = __kfGroups[@ 0];
		var kfmyops = kfgrp.__kfMyGroupOptions;
		
		buffer_async_group_begin(kfmyops.__kfGroupName);
		
		// those options are required for every group:
		buffer_async_group_option("slottitle", kfmyops.__kfSlotTitle);
		buffer_async_group_option("subtitle", kfmyops.__kfSubTitle);
		buffer_async_group_option("showdialog", kfmyops.__kfShowDialog);
		__kfTrace("Saving to {}", [ kfmyops.__kfSlotTitle ]);
		
		if (__kfIsPSOrbEro) {
			if (string_count(" ", kfmyops.__kfSlotTitle) > 0 || string_count(" ", kfmyops.__kfSubTitle) > 0) {
				__kfTrace("WARNING: PS4 and PS5 are very tricky when it comes to spaces in titles!");
				// ideally this should be a __kfThrow but uhhh not sure about that...
			}
			
			buffer_async_group_option("ps_create_backup", kfmyops.__kfPSCreateBackup);
			__kfTrace("Setting PS backup option to={}", [ kfmyops.__kfPSCreateBackup ]);
		}
		
		if (kfmyops.__kfSavePadIndex > -1) {
			buffer_async_group_option("savepadindex", kfmyops.__kfSavePadIndex);
			__kfTrace("Setting save pad index to={}", [ kfmyops.__kfSavePadIndex ]);
		}
		
		if (kfmyops.__kfAccountIndex > -1) {
			buffer_async_group_option("accountindex", kfmyops.__kfAccountIndex);
			__kfTrace("Setting account index to={}", [ kfmyops.__kfAccountIndex ]);
		}
		
		var kfissave = __kfGroups[@ 0].__kfIsSave;
		for (var kfi = 0; kfi < array_length(kfgrp.__kfMyFiles); ++kfi) {
			var kfdat = kfgrp.__kfMyFiles[@ kfi];
			if (kfissave) {
				buffer_save_async(kfdat.__kfBuffer, kfdat.__kfFileName, kfdat.__kfFileOffs, kfdat.__kfFileSize);
			}
			else {
				buffer_load_async(kfdat.__kfBuffer, kfdat.__kfFileName, kfdat.__kfFileOffs, kfdat.__kfFileSize);	
			}
		}
		
		var kfgrpid = buffer_async_group_end();
		__kfGroups[@ 0].__kfMyId = kfgrpid;
		
		return kfgrpid;
	};
	
	if (variable_global_exists("__kfReinitGuard")) {
		/* ... sigh */
		__kfThrow("Kotfile has already been initialized, are you using game_restart()?");	
	}
	/* KF PRIVATE METHODS END */

	/* KF PUBLIC METHODS START */
	/* public methods are NOT prefixed */
	groups = function() {
		// returns the amount of groups Kotfile has queued,
		// a value of 0 means it has no groups queued and is completely idle.
		return array_length(__kfGroups);
	};
	
	isInGroup = function() {
		// returns `true` if we're currently in startGroup(), `false` otherwise.
		return !is_undefined(__kfCurrentGroup);	
	};
	
	makeBuff = function(argContentsStringOpt) {
		// a small helper for making buffers.
		if (is_undefined(argContentsStringOpt)) {
			// just make a typical 1,grow,1 buffer
			return buffer_create(1, buffer_grow, 1);	
		}
		else {
			// make a buffer and write a null-term string to it.
			var kfbcontents = string(argContentsStringOpt);
			var kfbsize = string_byte_length(kfbcontents) + 1;
			// grow since you might append something to it later...
			var kfb = buffer_create(kfbsize, buffer_grow, 1);
			buffer_write(kfb, buffer_string, kfbcontents);
			// the seek position is after the string...
			return kfb;
		}
	};
	
	startGroup = function(argIsSaveBoolOpt) {
		if (!is_undefined(__kfCurrentGroup)) {
			__kfThrow("Already building an async group...");	
		}
		
		var kfisSaveGroup = is_undefined(argIsSaveBoolOpt) ? false : bool(argIsSaveBoolOpt);
		
		__kfTrace("Async group start at {}", [ __kfDateTimer() ]);
		__kfCurrentGroup = {
			__kfMyId: -1, // id of the group
			__kfIsSave: kfisSaveGroup,
			__kfMyFiles: [ /* queueFile() stuff will be here */ ],
			__kfMyGroupOptions: {
				__kfGroupName:      groupName,
				__kfSlotTitle:      slotTitle,
				__kfSubTitle:       subTitle,
				__kfShowDialog:     showDialog,
				__kfPSCreateBackup: psCreateBackup,
				__kfSavePadIndex:   savePadIndex,
				__kfAccountIndex:   accountIndex
			}
		};
		
		return self;
	};
	
	setGroupOption = function(argOptionNameString, argOptionValueAny, argPreviousValueRefOpt) {
		if (is_undefined(argOptionNameString)) {
			__kfThrow("Required argument argOptionNameString is undefined.");
		}
		
		if (is_undefined(argOptionValueAny)) {
			__kfThrow("Required argument argOptionValueAny is undefined.");
		}
		
		var kfdoprev = !is_undefined(argPreviousValueRefOpt);
		
		var kfgroupopt = string(argOptionNameString);
		
		__kfTrace("Overriding group option {} to {}.", [ kfgroupopt, string(argOptionValueAny) ]);
		
		switch (kfgroupopt) {
			case "groupName": {
				if (kfdoprev) argPreviousValueRefOpt.val = groupName;
				groupName = string(argOptionValueAny);
				break;
			}
			
			case "slotTitle": {
				if (kfdoprev) argPreviousValueRefOpt.val = slotTitle;
				slotTitle = string(argOptionValueAny);
				break;
			}
			
			case "subTitle": {
				if (kfdoprev) argPreviousValueRefOpt.val = subTitle;
				subTitle = string(argOptionValueAny);
				break;
			}
			
			case "showDialog": {
				if (kfdoprev) argPreviousValueRefOpt.val = showDialog;
				showDialog = bool(argOptionValueAny);
				break;
			}
			
			case "psCreateBackup": {
				if (kfdoprev) argPreviousValueRefOpt.val = psCreateBackup;
				psCreateBackup = bool(argOptionValueAny);
				break;
			}
			
			case "savePadIndex": {
				if (kfdoprev) argPreviousValueRefOpt.val = savePadIndex;
				savePadIndex = real(argOptionValueAny);
				break;
			}
			
			case "accountIndex": {
				if (kfdoprev) argPreviousValueRefOpt.val = accountIndex;
				accountIndex = real(argOptionValueAny);
				break;	
			}
			
			default: {
				// uhhh what do I even set here...? whoops...
				throw __kfThrow("Unrecognized group option = {}", [ kfgroupopt ]);
				break;
			}
		}
		
		return self;
	};
	
	queueFile = function(argFileNameString, argIsSaveBool, argOnCallMethodOpt, argBufferIndexOpt, argFileOffsetRealOpt, argFileSizeRealOpt, argUserDataOpt) {
		if (is_undefined(argFileNameString)) {
			__kfThrow("Required argument argFileNameString not provided");
		}
		
		var kfissave  = is_undefined(argIsSaveBool) ? __kfThrow("Required argument argIsSaveBool not provided") : argIsSaveBool;
		var kfnogroup = false;
		
		if (is_undefined(__kfCurrentGroup)) {
			if (allowImplicitGroups) {
				kfnogroup = true;
				__kfTrace("Implicit group begin. issave={}", [ kfissave ]);
				startGroup(kfissave);
			}
			else {
				__kfThrow("No group is defined, and implicit groups are not allowed.");	
			}
		}
		
		if (__kfCurrentGroup.__kfIsSave != kfissave) {
			__kfThrow("Non-matching group type. groupIsSave={} queueIsSave={}", [ __kfCurrentGroup.__kfIsSave, kfissave ]);	
		}
		
		var fname = string(argFileNameString);
		var foffs = is_undefined(argFileOffsetRealOpt) ?  0 : argFileOffsetRealOpt;
		var fsize = is_undefined(argFileSizeRealOpt  ) ? -1 : argFileSizeRealOpt;
		var ffunc = argOnCallMethodOpt; // can be either a method or a script index o_O
		var fuser = argUserDataOpt; // can be anything...
		var fbuff = is_undefined(argBufferIndexOpt) ? makeBuff() : argBufferIndexOpt;
		var fasid = -1;
		
		// if buffer size is -1 (or 0, which is nonsense) and we're saving, just guess...
		if (kfissave && fsize < 1) {
			fsize = buffer_get_size(fbuff) - foffs;	
		}
		
		var kfdat = {
			__kfFileName: fname,
			__kfFileOffs: foffs,
			__kfFileSize: fsize,
			__kfOnCall:   ffunc,
			__kfUserData: fuser,
			__kfIsSave:   kfissave,
			__kfBuffer:   fbuff
		};
		
		array_push(__kfCurrentGroup.__kfMyFiles, kfdat);
		
		if (kfnogroup) {
			endGroup();
			__kfTrace("Implicit group end.");
		}
		
		return self;
	};
	
	endGroup = function() {
		if (is_undefined(__kfCurrentGroup)) {
			__kfThrow("No async group was made to begin with...");
		}
		
		/* check if we have no groups currently processing,
		 * if we do, queue manually
		 * if we don't, start GM fun, let the Async Load event continue
		 */
		var kfneedpush = groups() == 0;
		
		array_push(__kfGroups, __kfCurrentGroup);
		__kfCurrentGroup = undefined;
		
		if (kfneedpush) {
			__kfDispatchGroup();
			__kfTrace("Starting group dispatch...");
		}
		else {
			__kfTrace("Queueing group...");	
		}
		
		return self;
	};
	/* KF PUBLIC METHODS END */
	
	/* KF INIT CODE START */
	/* you usually shouldn't touch this... */
	__kfTrace("Welcome to Kotfile by {}, this is version {} made on {}.", [ __kfAuthor, __kfVersion, __kfDate ]);
	global.__kfReinitGuard = __kfDateTimer();
	return "<kf dummy return, please ignore...>";
	/* KF INIT CODE END */
}

/// @description Kotfile: Handles the Asynchronous Save Load event.
/// @param {Mixed} [argAsyncLoadMapOpt] a ds map id or nothing to use async_load.
/// @context {KotfileInit}
function KotfileAsync(argAsyncLoadMapOpt) {
	var kfm = is_undefined(argAsyncLoadMapOpt) ? async_load : argAsyncLoadMapOpt;
	var kfid = kfm[? "id"];
	var kfstatus = kfm[? "status"]; // this status is usually a lie.
	var kferror = kfm[? "error"]; // also don't trust that one.
	var kfok = false;
	var kfarrind = -1;
	
	for (var i = 0; i < array_length(__kfGroups); ++i) {
		/* ignore the groups we don't need */
		if (kfid != __kfGroups[@ i].__kfMyId) continue;
		
		/* mark our group */
		kfarrind = i;
		kfok = true;
		break;
	}
	
	if (!kfok) {
		__kfTrace("Async group id={} is not recognized. (outside load thing?)", [ kfid ]);
		return -1;
	}

	/* handle our group */
	var kfissave = __kfGroups[@ kfarrind].__kfIsSave;
	var kfmygrp = __kfGroups[@ kfarrind].__kfMyFiles;
	for (var kffile = 0; kffile < array_length(kfmygrp); ++kffile) {
		/* just in case you don't want any callbacks, when saving I guess... */
		var kfmyfile = kfmygrp[@ kffile];
		var kfdofreebuff = true;
		if (!is_undefined(kfmyfile.__kfOnCall)) {
			var kfcbret = kfmyfile.__kfOnCall({
				isSave:      kfissave,
				gmStatus:    kfstatus,
				gmError:     kferror,
				userData:    kfmyfile.__kfUserData,
				fileName:    kfmyfile.__kfFileName,
				bufferIndex: kfmyfile.__kfBuffer
			});
			
			if (!is_undefined(kfcbret) && kfcbret) {
				kfdofreebuff = false;
			}
		}
		
		if (kfdofreebuff && buffer_exists(kfmyfile.__kfBuffer)) {
			__kfTrace("Freeing buffer {}.", [ kfmyfile.__kfBuffer ]);
			buffer_delete(kfmyfile.__kfBuffer);
		}
	}
	kfmygrp = undefined;
	
	/* some other post-save handling code... */
	if (kfissave) {
		/* If you do not own the Switch export module, comment out this whole block just fine... */
		if (__kfIsSwitch) {
			/*
				For those who don't know, this function takes no arguments
				and always returns -1 lmfao
				Needed on a Switch after every save event to flush the journal to the sd card (and cloud save)
			*/
			var kfswitchcommitres = switch_save_data_commit();
			__kfTrace("We're on a Switch and in a save group, commit={}...", [ kfswitchcommitres ]);
		}
	}
	
	/* clean up the group */
	__kfTrace("Async group id {} DONE at array index {}.", [ kfid, kfarrind ]);
	array_delete(__kfGroups, kfarrind, 1);
	
	/* continue with the next group...? */
	__kfDispatchGroup();
	
	/* return the group id we just handled. */
	return kfid;
}
