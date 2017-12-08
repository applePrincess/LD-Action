APPNAME = LDAct8
CC		= csc
CFLAGS	= /d:HAISHIN

all:
	$(CC) $(CFLAGS) $(APPNAME).cs MyForm.cs

release:
	$(CC) /target:winexe $(APPNAME).cs MyForm.cs

start:
	$(APPNAME).exe
