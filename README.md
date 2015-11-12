#QR Code All
This KeePass plugin provides a QR code rendering of a selected entry password or username/password/url/note.

ENV:
========
1. VC# Express 2010
2. KeePass 2.30 ( so, if you want to using it on older KeePass, you must compiling it youself)
3. ZXing.net
4. NGetText (for locale zh_CN/zh_TW)
5. QrCodeGenerator-2.0.12 (get it from KeePass plugins web page)

Note:
========
Some code based on ZXing.net demo and QrCodeGenerator-2.0.12

Icon:
========
Polaris UI and Linecons

Polaris UI and Linecons was specially made for Smashing Magazine by our friends Designmodo.com. 

Thanks for supporting our website and enjoy!

Updates:
http://designmodo.com/polaris-free
http://designmodo.com/linecons-free


download:
=========
https://bitbucket.org/netcharm/qrcodeall/downloads
1. for keepass 2.1+, you may download "*.plgx" file.
2. if you using keepass 2.30+, you can download pre-compiled "*.7z" file

Changes:
=========
1.0.5     - add locale feature via NGetText (using standard gettext po file and mo file)

1.0.3     - fixed some bug, such as utf-8 encoding, limited text length to encoding

1.0.0     - Initial public release