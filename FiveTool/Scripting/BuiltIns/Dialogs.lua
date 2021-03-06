﻿local BuildFilterList

function ChooseGameFolder()
	return Dialog.OpenGameFolder()
end

DefineHelp ("File", "ChooseGameFolder", {
	shortDescription = "Choose a different game folder to use",
	longDescription = "Asks the user to choose where their game files are installed.\nAll functions which accept file paths will use this folder as a base.",
	returns = "true if the user selected a folder",
	examples = {
		"ChooseGameFolder()",
	},
});

function ChooseFile(extension, title)
	if title == nil then
		title = "Open File"
	end
	return Dialog.OpenFile({
		type = "open",
		title = title,
		filters = BuildFilterList(extension),
	})
end

DefineHelp("File", "ChooseFile", {
	shortDescription = "Browse for a file",
	longDescription = "Shows a dialog allowing the user to select a file.\nThe file will be made available for script use regardless of its location.",
	args = {
		{ "extension", "(Optional) The file extension to filter with, e.g. \"module\"." },
		{ "title", "(Optional) The dialog title" },
	},
	returns = "A unique path token that refers to the file, or nil if the user cancelled",
	examples = {
		"ChooseFile()",
		"ChooseFile(\"module\")",
		"ChooseFile(\"module\", \"Open Module File\")",
	},
})

function ChooseNewFile(extension, title, filename)
	if title == nil then
		title = "Save As"
	end
	return Dialog.OpenFile({
		type = "save",
		title = title,
		filters = BuildFilterList(extension),
		suggestedName = filename,
	})
end

DefineHelp("File", "ChooseNewFile", {
	shortDescription = "Browse for a file to create",
	longDescription = "Shows a \"Save As\" dialog allowing the user to create a file.\nThe file will be made available for script use regardless of its location.",
	args = {
		{ "extension", "(Optional) The file extension to filter with, e.g. \"module\"." },
		{ "title", "(Optional) The dialog title" },
		{ "filename", "(Optional) The suggested filename to display" },
	},
	returns = "A unique path token that refers to the file, or nil if the user canceled",
	examples = {
		"ChooseNewFile()",
		"ChooseNewFile(\"txt\")",
		"ChooseNewFile(\"txt\", \"Save Output As\")",
		"ChooseNewFile(\"txt\", \"Save Output As\", \"out.txt\")",
	},
})

function ChooseFolder(message)
	if message == nil then
		message = "Select a folder."
	end
	return Dialog.OpenFolder({
		message = message,
		allowCreate = false,
	})
end

DefineHelp("File", "ChooseFolder", {
	shortDescription = "Browse for a folder",
	longDescription = "Shows a dialog asking the user to browse for a folder.\nThis will grant the script access to every file and subfolder of the folder.",
	args = {
		{ "message", "(Optional) The message to display in the dialog" },
	},
	returns = "A unique path token that refers to the directory, or nil if the user canceled. By appending to the returned string, you can create paths for files in the folder.",
	examples = {
		"ChooseFolder()",
		"ChooseFolder(\"Select the folder containing the tags to import.\")",
		"io.open(ChooseFolder() .. \"data.txt\")",
	},
})

function ChooseNewFolder(message)
	if message == nil then
		message = "Select a folder."
	end
	return Dialog.OpenFolder({
		message = message,
		allowCreate = true,
	})
end

DefineHelp("File", "ChooseNewFolder", {
	shortDescription = "Browse for a folder which can be created",
	longDescription = "Shows a dialog asking the user to browse for a folder.\nThere will be an option for the user to create a new folder.\nThis will grant the script access to every file and subfolder of the folder.",
	args = {
		{ "message", "(Optional) The message to display in the dialog" },
	},
	returns = "A unique path token that refers to the directory, or nil if the user canceled. By appending to the returned string, you can create paths for files in the folder.",
	examples = {
		"ChooseNewFolder()",
		"ChooseNewFolder(\"Select the folder to save extracted files to.\")",
		"io.open(ChooseNewFolder() .. \"out.txt\", \"w\")",
	},
})

BuildFilterList = function(extension)
	local filters = {
		{ "All Files", "*.*" },
	}
	if extension ~= nil then
		local name = extension:sub(1, 1):upper() .. extension:sub(2) -- Capitalize the name
		table.insert(filters, 1, { name .. " Files", "*." .. extension })
	end
	return filters
end