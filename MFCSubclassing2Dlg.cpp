
// MFCSubclassing2Dlg.cpp : implementation file
//

#include "stdafx.h"
#include "MFCSubclassing2.h"
#include "MFCSubclassing2Dlg.h"
#include "afxdialogex.h"

#ifdef _DEBUG
#define new DEBUG_NEW
#endif


// CMFCSubclassing2Dlg dialog



CMFCSubclassing2Dlg::CMFCSubclassing2Dlg(CWnd* pParent /*=nullptr*/)
	: CDialogEx(IDD_MFCSUBCLASSING2_DIALOG, pParent)
{
	m_hIcon = AfxGetApp()->LoadIcon(IDR_MAINFRAME);
}

void CMFCSubclassing2Dlg::DoDataExchange(CDataExchange* pDX)
{
	CDialogEx::DoDataExchange(pDX);
}

BEGIN_MESSAGE_MAP(CMFCSubclassing2Dlg, CDialogEx)
	ON_WM_PAINT()
	ON_WM_QUERYDRAGICON()
	ON_BN_CLICKED(IDC_INC_BTN, &CMFCSubclassing2Dlg::OnBnClickedIncBtn)
	ON_BN_CLICKED(IDC_DEC_BTN, &CMFCSubclassing2Dlg::OnBnClickedDecBtn)
END_MESSAGE_MAP()


// CMFCSubclassing2Dlg message handlers

BOOL CMFCSubclassing2Dlg::OnInitDialog()
{
	CDialogEx::OnInitDialog();

	// Set the icon for this dialog.  The framework does this automatically
	//  when the application's main window is not a dialog
	SetIcon(m_hIcon, TRUE);			// Set big icon
	SetIcon(m_hIcon, FALSE);		// Set small icon

	SetDlgItemInt(IDC_VALUE_EDIT, 0);
	m_inc_btn.SubclassDlgItem(IDC_INC_BTN, this);
	m_dec_btn.SubclassDlgItem(IDC_DEC_BTN, this);

	return TRUE;  // return TRUE  unless you set the focus to a control
}

// If you add a minimize button to your dialog, you will need the code below
//  to draw the icon.  For MFC applications using the document/view model,
//  this is automatically done for you by the framework.

void CMFCSubclassing2Dlg::OnPaint()
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
HCURSOR CMFCSubclassing2Dlg::OnQueryDragIcon()
{
	return static_cast<HCURSOR>(m_hIcon);
}



void CMFCSubclassing2Dlg::OnBnClickedIncBtn()
{
	int value = GetDlgItemInt(IDC_VALUE_EDIT);
	SetDlgItemInt(IDC_VALUE_EDIT, value + 1);
}


void CMFCSubclassing2Dlg::OnBnClickedDecBtn()
{
	int value = GetDlgItemInt(IDC_VALUE_EDIT);
	SetDlgItemInt(IDC_VALUE_EDIT, value - 1);
}


BOOL CMFCSubclassing2Dlg::OnCommand(WPARAM wParam, LPARAM lParam)
{
	if (HIWORD(wParam) == 20000)
	{
		int value = GetDlgItemInt(IDC_VALUE_EDIT);
		if (LOWORD(wParam) == IDC_INC_BTN) value++;
		else value--;

		SetDlgItemInt(IDC_VALUE_EDIT, value);
	}

	return CDialogEx::OnCommand(wParam, lParam);
}
