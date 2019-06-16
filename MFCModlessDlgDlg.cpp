
// MFCModlessDlgDlg.cpp : implementation file
//

#include "stdafx.h"
#include "MFCModlessDlg.h"
#include "MFCModlessDlgDlg.h"
#include "afxdialogex.h"

#ifdef _DEBUG
#define new DEBUG_NEW
#endif


// CMFCModlessDlgDlg dialog



CMFCModlessDlgDlg::CMFCModlessDlgDlg(CWnd* pParent /*=nullptr*/)
	: CDialogEx(IDD_MFCMODLESSDLG_DIALOG, pParent)
{
	m_hIcon = AfxGetApp()->LoadIcon(IDR_MAINFRAME);
}

void CMFCModlessDlgDlg::DoDataExchange(CDataExchange* pDX)
{
	CDialogEx::DoDataExchange(pDX);
}

BEGIN_MESSAGE_MAP(CMFCModlessDlgDlg, CDialogEx)
	ON_WM_PAINT()
	ON_WM_QUERYDRAGICON()
	ON_BN_CLICKED(IDC_SHOW_BTN, &CMFCModlessDlgDlg::OnBnClickedShowBtn)
	ON_WM_DESTROY()
	ON_MESSAGE(27000, &CMFCModlessDlgDlg::On27000)
	ON_BN_CLICKED(IDC_SET_BTN, &CMFCModlessDlgDlg::OnBnClickedSetBtn)
	ON_MESSAGE(27001, &CMFCModlessDlgDlg::On27001)
END_MESSAGE_MAP()


// CMFCModlessDlgDlg message handlers

BOOL CMFCModlessDlgDlg::OnInitDialog()
{
	CDialogEx::OnInitDialog();

	// Set the icon for this dialog.  The framework does this automatically
	//  when the application's main window is not a dialog
	SetIcon(m_hIcon, TRUE);			// Set big icon
	SetIcon(m_hIcon, FALSE);		// Set small icon
	
	mp_my_view = new MyView;
	mp_my_view->Create(IDD_MY_VIEW, NULL);
	mp_my_view->SetWindowPos(NULL, 0, 120, 0, 0, SWP_NOSIZE);

	return TRUE;  // return TRUE  unless you set the focus to a control
}

// If you add a minimize button to your dialog, you will need the code below
//  to draw the icon.  For MFC applications using the document/view model,
//  this is automatically done for you by the framework.

void CMFCModlessDlgDlg::OnPaint()
{
	if (IsIconic())
	{
		CPaintDC dc(this); // device context for painting

		SendMessage(WM_ICONERASEBKGND, reinterpret_cast<WPARAM>(dc.GetSafeHdc()), 0);

		// Center icon in client rectangle
		int cxIcon = GetSystemMetrics(SM_CXICON);
		int cyIcon = GetSystemMetrics(SM_CYICON);
		CRect rect;
		GetClientRect(&rect);
		int x = (rect.Width() - cxIcon + 1) / 2;
		int y = (rect.Height() - cyIcon + 1) / 2;

		// Draw the icon
		dc.DrawIcon(x, y, m_hIcon);
	}
	else
	{
		CDialogEx::OnPaint();
	}
}

// The system calls this function to obtain the cursor to display while the user drags
//  the minimized window.
HCURSOR CMFCModlessDlgDlg::OnQueryDragIcon()
{
	return static_cast<HCURSOR>(m_hIcon);
}



void CMFCModlessDlgDlg::OnBnClickedShowBtn()
{
	if (mp_new_dlg == NULL)
	{
		mp_new_dlg = new NewDlg;
		mp_new_dlg->Create(IDD_NEW_DLG, this); // shows on taskbar(application window)
	}
}


void CMFCModlessDlgDlg::OnDestroy()
{
	CDialogEx::OnDestroy();

	if (mp_new_dlg != NULL)
	{
		On27000(0, 0);
		//mp_new_dlg->DestroyWindow();
		//delete mp_new_dlg;
		//mp_new_dlg = NULL;
	}

	mp_my_view->DestroyWindow();
	delete mp_my_view;

}


afx_msg LRESULT CMFCModlessDlgDlg::On27000(WPARAM wParam, LPARAM lParam)
{
	mp_new_dlg->DestroyWindow();
	delete mp_new_dlg;
	mp_new_dlg = NULL;

	return 0;
}


void CMFCModlessDlgDlg::OnBnClickedSetBtn()
{
	if (mp_new_dlg != NULL)
	{
		int	num = GetDlgItemInt(IDC_NUM_EDIT);
		mp_new_dlg->SetDlgItemInt(IDC_NUM_EDIT, num);
	}
}


afx_msg LRESULT CMFCModlessDlgDlg::On27001(WPARAM wParam, LPARAM lParam)
{
	int num = (int)wParam;
	if (num < 0) num = num * (-1);
	SetDlgItemInt(IDC_NUM_EDIT, num);
	return 0;
}
