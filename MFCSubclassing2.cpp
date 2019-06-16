
// MFCSubclassing2.cpp : Defines the class behaviors for the application.
//

#include "stdafx.h"
#include "MFCSubclassing2.h"
#include "MFCSubclassing2Dlg.h"

#ifdef _DEBUG
#define new DEBUG_NEW
#endif


// CMFCSubclassing2App

BEGIN_MESSAGE_MAP(CMFCSubclassing2App, CWinApp)
	ON_COMMAND(ID_HELP, &CWinApp::OnHelp)
END_MESSAGE_MAP()


// CMFCSubclassing2App construction

CMFCSubclassing2App::CMFCSubclassing2App()
{
	// TODO: add construction code here,
	// Place all significant initialization in InitInstance
}


// The one and only CMFCSubclassing2App object

CMFCSubclassing2App theApp;


// CMFCSubclassing2App initialization

BOOL CMFCSubclassing2App::InitInstance()
{
	
	CWinApp::InitInstance();

	CMFCSubclassing2Dlg dlg;
	m_pMainWnd = &dlg;
	dlg.DoModal();

	return FALSE;
}

