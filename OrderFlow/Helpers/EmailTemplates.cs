namespace OrderFlow.Helpers
{
    public class EmailTemplates
    {
        private const string GradientStart = "#0d6efd";
        private const string GradientEnd = "#198754";

        public static string ResetPassword(string resetLink, string privacyPolicyLink)
        {

            string page = $"""
                                <!DOCTYPE html>
                                <html lang="en">
                                <head><meta charset="UTF-8"><meta name="viewport" content="width=device-width,initial-scale=1"></head>
                                <body style="margin:0;padding:0;background:#f4f4f4;font-family:'Helvetica Neue',Arial,sans-serif;">
                                  <table width="100%" cellpadding="0" cellspacing="0" style="padding:40px 16px;">
                                    <tr><td align="center">
                                      <table width="600" cellpadding="0" cellspacing="0"
                                             style="max-width:600px;width:100%;background:#ffffff;border-radius:12px;
                                                    overflow:hidden;box-shadow:0 0.5rem 1rem rgba(0,0,0,0.15);">

                                        <tr>
                                          <td style="background:#0a0a0a;padding:32px 40px;">
                                            <table width="100%" cellpadding="0" cellspacing="0"><tr>
                                              <td style="font-family:Georgia,serif;font-size:24px;font-weight:700;">
                                                <!-- CSS gradients on text are stripped by Gmail/Outlook, use two spans instead -->
                                                <span style="color:{GradientStart};">Order</span><span style="color:{GradientEnd};">Flow</span>
                                              </td>
                                              <td align="right" style="font-size:11px;color:#555555;letter-spacing:1.5px;text-transform:uppercase;">
                                                Password Reset
                                              </td>
                                            </tr></table>
                                          </td>
                                        </tr>

                                        <tr>
                                          <td style="height:4px;background:linear-gradient(to right,{GradientStart},{GradientEnd});
                                                     line-height:4px;font-size:4px;">&nbsp;</td>
                                        </tr>

                                        <tr>
                                          <td style="padding:48px 40px 32px;background:#ffffff;">
                                            <p style="margin:0 0 8px;font-size:13px;font-weight:600;letter-spacing:1px;
                                                       text-transform:uppercase;color:{GradientStart};">Security Notice</p>
                                            <h1 style="margin:0 0 20px;font-size:28px;color:#0a0a0a;font-weight:700;
                                                        line-height:1.2;font-family:Georgia,serif;">
                                              Reset your<br>password
                                            </h1>
                                            <p style="margin:0 0 28px;font-size:15px;color:#444444;line-height:1.7;">
                                              We received a request to reset the password for your <strong>OrderFlow</strong> account.
                                              Click the button below to set a new password. This link will expire in <strong>2 hours</strong>.
                                            </p>

                                            <!-- CTA Button — gradient background works fine here (it's on a div, not text) -->
                                            <table cellpadding="0" cellspacing="0" style="margin:0 0 32px;">
                                              <tr>
                                                <td style="background:linear-gradient(to right,{GradientStart},{GradientEnd});
                                                           border-radius:8px;box-shadow:0 0.5rem 1rem rgba(0,0,0,0.15);">
                                                  <a href="{resetLink}"
                                                     style="display:block;padding:16px 44px;color:#ffffff;text-decoration:none;
                                                            font-size:15px;font-weight:600;letter-spacing:0.3px;">
                                                    Reset Password &#8594;
                                                  </a>
                                                </td>
                                              </tr>
                                            </table>

                                            <table width="100%" cellpadding="0" cellspacing="0"
                                                   style="background:#f8f9fa;border:1px solid #e9ecef;border-radius:8px;margin-bottom:28px;">
                                              <tr>
                                                <td style="padding:16px 20px;">
                                                  <p style="margin:0 0 4px;font-size:12px;color:#888888;
                                                             text-transform:uppercase;letter-spacing:1px;">Or copy this link</p>
                                                  <p style="margin:0;font-size:12px;color:{GradientStart};word-break:break-all;
                                                             font-family:'Courier New',monospace;">{resetLink}</p>
                                                </td>
                                              </tr>
                                            </table>

                                            <table width="100%" cellpadding="0" cellspacing="0"
                                                   style="border-top:1px solid #f0f0f0;">
                                              <tr>
                                                <td style="padding-top:20px;">
                                                  <p style="margin:0;font-size:13px;color:#888888;line-height:1.6;">
                                                    If you didn&#39;t request a password reset, you can safely ignore this email —
                                                    your password will remain unchanged. If you&#39;re concerned about your
                                                    account&#39;s security, please contact our support team.
                                                  </p>
                                                </td>
                                              </tr>
                                            </table>
                                          </td>
                                        </tr>
                                        <tr>
                                          <td style="background:#0a0a0a;padding:24px 40px;">
                                            <table width="100%" cellpadding="0" cellspacing="0"><tr>
                                              <td style="font-size:12px;color:#555555;">&#169; 2026 OrderFlow. All rights reserved.</td>
                                              <td align="right">
                                                <a href="{privacyPolicyLink}" style="font-size:12px;color:#555555;text-decoration:none;margin-left:16px;">Privacy</a>
                                              </td>
                                            </tr></table>
                                          </td>
                                        </tr>

                                      </table>
                                    </td></tr>
                                  </table>
                                </body>
                                </html>
                              """;

            return page;
        }
    }

}

