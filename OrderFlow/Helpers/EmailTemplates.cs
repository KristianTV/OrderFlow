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

        public static string OrderCompleted(
            string customerName,
            string orderId,
            string orderDate,
            string orderLink,
            string privacyPolicyLink)
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
                                                <span style="color:{GradientStart};">Order</span><span style="color:{GradientEnd};">Flow</span>
                                              </td>
                                              <td align="right" style="font-size:11px;color:#555555;letter-spacing:1.5px;text-transform:uppercase;">
                                                Order Confirmed
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
                                                       text-transform:uppercase;color:{GradientEnd};">Order Complete</p>
                                            <h1 style="margin:0 0 16px;font-size:28px;color:#0a0a0a;font-weight:700;
                                                        line-height:1.2;font-family:Georgia,serif;">
                                              Thank you,<br>{customerName}!
                                            </h1>
                                            <p style="margin:0 0 28px;font-size:15px;color:#444444;line-height:1.7;">
                                              Your order has been confirmed and is being prepared for dispatch.
                                              You&#39;ll receive a shipping notification once it&#39;s on its way.
                                            </p>

                                            <table width="100%" cellpadding="0" cellspacing="0"
                                                   style="background:#f8f9fa;border:1px solid #e9ecef;border-radius:8px;margin-bottom:32px;">
                                              <tr>
                                                <td style="padding:16px 20px;">
                                                  <table width="100%" cellpadding="0" cellspacing="0">
                                                    <tr>
                                                      <td style="font-size:12px;color:#888888;text-transform:uppercase;
                                                                 letter-spacing:1px;padding-bottom:4px;">Order ID</td>
                                                      <td align="right" style="font-size:12px;color:#888888;text-transform:uppercase;
                                                                               letter-spacing:1px;padding-bottom:4px;">Date</td>
                                                    </tr>
                                                    <tr>
                                                      <td style="font-size:14px;font-weight:600;color:{GradientStart};
                                                                 font-family:'Courier New',monospace;">{orderId}</td>
                                                      <td align="right" style="font-size:14px;color:#444444;">{orderDate}</td>
                                                    </tr>
                                                  </table>
                                                </td>
                                              </tr>
                                            </table>

                                            <table cellpadding="0" cellspacing="0" style="margin:0 0 32px;">
                                              <tr>
                                                <td style="border:2px solid #0a0a0a;border-radius:8px;">
                                                  <a href="{orderLink}"
                                                     style="display:block;padding:14px 44px;color:#0a0a0a;text-decoration:none;
                                                            font-size:15px;font-weight:600;letter-spacing:0.3px;">
                                                    View Order Details
                                                  </a>
                                                </td>
                                              </tr>
                                            </table>

                                            <table width="100%" cellpadding="0" cellspacing="0"
                                                   style="border-top:1px solid #f0f0f0;">
                                              <tr>
                                                <td style="padding-top:20px;">
                                                  <p style="margin:0;font-size:13px;color:#888888;line-height:1.6;">
                                                    Questions about your order? Reply to this email or visit our
                                                    support centre and quote your order ID.
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

        public static string PaymentRequired(
            string customerName,
            string orderId,
            string amount,
            string orderLink,
            string privacyPolicyLink)
        {
            const string WarnColor = "#e67e00";

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
                                                <span style="color:{GradientStart};">Order</span><span style="color:{GradientEnd};">Flow</span>
                                              </td>
                                              <td align="right" style="font-size:11px;color:#555555;letter-spacing:1.5px;text-transform:uppercase;">
                                                Action Required
                                              </td>
                                            </tr></table>
                                          </td>
                                        </tr>

                                        <tr>
                                          <td style="height:4px;background:linear-gradient(to right,{WarnColor},{GradientStart});
                                                     line-height:4px;font-size:4px;">&nbsp;</td>
                                        </tr>

                                        <tr>
                                          <td style="padding:48px 40px 32px;background:#ffffff;">
                                            <p style="margin:0 0 8px;font-size:13px;font-weight:600;letter-spacing:1px;
                                                       text-transform:uppercase;color:{WarnColor};">Payment Pending</p>
                                            <h1 style="margin:0 0 16px;font-size:28px;color:#0a0a0a;font-weight:700;
                                                        line-height:1.2;font-family:Georgia,serif;">
                                              Complete your<br>payment
                                            </h1>

                                            <table width="100%" cellpadding="0" cellspacing="0"
                                                   style="background:#fff8f0;border:1px solid #fde3c0;border-radius:8px;margin-bottom:28px;">
                                              <tr>
                                                <td style="padding:20px 24px;">
                                                  <table width="100%" cellpadding="0" cellspacing="0">
                                                    <tr>
                                                      <td>
                                                        <p style="margin:0 0 4px;font-size:12px;color:#888888;
                                                                   text-transform:uppercase;letter-spacing:1px;">Order ID</p>
                                                        <p style="margin:0;font-size:14px;font-weight:600;color:{GradientStart};
                                                                   font-family:'Courier New',monospace;">{orderId}</p>
                                                      </td>
                                                      <td align="right">
                                                        <p style="margin:0 0 4px;font-size:12px;color:#888888;
                                                                   text-transform:uppercase;letter-spacing:1px;">Amount</p>
                                                        <p style="margin:0;font-size:24px;font-weight:700;color:{WarnColor};">{amount}</p>
                                                      </td>
                                                    </tr>
                                                  </table>
                                                </td>
                                              </tr>
                                            </table>

                                            <table cellpadding="0" cellspacing="0" style="margin:0 0 32px;">
                                              <tr>
                                                <td style="border:2px solid #0a0a0a;border-radius:8px;">
                                                  <a href="{orderLink}"
                                                     style="display:block;padding:14px 44px;color:#0a0a0a;text-decoration:none;
                                                            font-size:15px;font-weight:600;letter-spacing:0.3px;">
                                                    View Order Details
                                                  </a>
                                                </td>
                                              </tr>
                                            </table>

                                            <table width="100%" cellpadding="0" cellspacing="0"
                                                   style="border-top:1px solid #f0f0f0;">
                                              <tr>
                                                <td style="padding-top:20px;">
                                                  <p style="margin:0;font-size:13px;color:#888888;line-height:1.6;">
                                                    If you believe you&#39;ve already paid, please contact our support
                                                    team quoting your order ID and we&#39;ll resolve this promptly.
                                                    If you no longer wish to proceed, you can ignore this email and
                                                    your order will be cancelled automatically.
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

        public static string PaymentSucceeded(
            string customerName,
            string orderId,
            string paymentDate,
            string amountPaid,
            string paymentMethod,
            string orderLink,
            string privacyPolicyLink)
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
                                                <span style="color:{GradientStart};">Order</span><span style="color:{GradientEnd};">Flow</span>
                                              </td>
                                              <td align="right" style="font-size:11px;color:#555555;letter-spacing:1.5px;text-transform:uppercase;">
                                                Payment Receipt
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
                                                       text-transform:uppercase;color:{GradientEnd};">Payment Confirmed</p>
                                            <h1 style="margin:0 0 16px;font-size:28px;color:#0a0a0a;font-weight:700;
                                                        line-height:1.2;font-family:Georgia,serif;">
                                              Payment received,<br>{customerName}!
                                            </h1>
                                            <p style="margin:0 0 28px;font-size:15px;color:#444444;line-height:1.7;">
                                              Your payment has been processed successfully. A receipt summary is below —
                                              keep it for your records.
                                            </p>

                                            <table width="100%" cellpadding="0" cellspacing="0"
                                                   style="background:#f0fdf4;border:1px solid #bbf7d0;border-radius:8px;margin-bottom:32px;">
                                              <tr>
                                                <td style="padding:24px;">
                                                  <!-- Amount paid — large focal number -->
                                                  <p style="margin:0 0 4px;font-size:12px;color:#888888;
                                                             text-transform:uppercase;letter-spacing:1px;">Amount Paid</p>
                                                  <p style="margin:0 0 20px;font-size:32px;font-weight:700;color:{GradientEnd};">{amountPaid}</p>

                                                  <table width="100%" cellpadding="0" cellspacing="0">
                                                    <tr>
                                                      <td style="padding:8px 0;font-size:13px;color:#888888;
                                                                 border-bottom:1px solid #d1fae5;">Order ID</td>
                                                      <td align="right" style="padding:8px 0;font-size:13px;font-weight:600;
                                                                               color:{GradientStart};font-family:'Courier New',monospace;
                                                                               border-bottom:1px solid #d1fae5;">{orderId}</td>
                                                    </tr>
                                                    <tr>
                                                      <td style="padding:8px 0;font-size:13px;color:#888888;
                                                                 border-bottom:1px solid #d1fae5;">Payment Method</td>
                                                      <td align="right" style="padding:8px 0;font-size:13px;color:#444444;
                                                                               border-bottom:1px solid #d1fae5;">{paymentMethod}</td>
                                                    </tr>
                                                    <tr>
                                                      <td style="padding:8px 0;font-size:13px;color:#888888;">Date</td>
                                                      <td align="right" style="padding:8px 0;font-size:13px;color:#444444;">{paymentDate}</td>
                                                    </tr>
                                                  </table>
                                                </td>
                                              </tr>
                                            </table>

                                            <table cellpadding="0" cellspacing="0" style="margin:0 0 32px;">
                                              <tr>
                                                <td style="background:linear-gradient(to right,{GradientStart},{GradientEnd});
                                                           border-radius:8px;box-shadow:0 0.5rem 1rem rgba(0,0,0,0.15);">
                                                  <a href="{orderLink}"
                                                     style="display:block;padding:16px 44px;color:#ffffff;text-decoration:none;
                                                            font-size:15px;font-weight:600;letter-spacing:0.3px;">
                                                    View Order &#8594;
                                                  </a>
                                                </td>
                                              </tr>
                                            </table>

                                            <table width="100%" cellpadding="0" cellspacing="0"
                                                   style="border-top:1px solid #f0f0f0;">
                                              <tr>
                                                <td style="padding-top:20px;">
                                                  <p style="margin:0;font-size:13px;color:#888888;line-height:1.6;">
                                                    If you did not authorise this payment or notice any discrepancy,
                                                    please contact our support team immediately quoting your transaction ID.
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

        public static string PaymentFailed(
            string customerName,
            string orderId,
            string amount,
            string failureReason,
            string privacyPolicyLink)
        {
            const string ErrorColor = "#dc3545";

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
                                                <span style="color:{GradientStart};">Order</span><span style="color:{GradientEnd};">Flow</span>
                                              </td>
                                              <td align="right" style="font-size:11px;color:#555555;letter-spacing:1.5px;text-transform:uppercase;">
                                                Payment Failed
                                              </td>
                                            </tr></table>
                                          </td>
                                        </tr>

                                        <tr>
                                          <td style="height:4px;background:linear-gradient(to right,{ErrorColor},{GradientStart});
                                                     line-height:4px;font-size:4px;">&nbsp;</td>
                                        </tr>

                                        <tr>
                                          <td style="padding:48px 40px 32px;background:#ffffff;">
                                            <p style="margin:0 0 8px;font-size:13px;font-weight:600;letter-spacing:1px;
                                                       text-transform:uppercase;color:{ErrorColor};">Payment Unsuccessful</p>
                                            <h1 style="margin:0 0 16px;font-size:28px;color:#0a0a0a;font-weight:700;
                                                        line-height:1.2;font-family:Georgia,serif;">
                                              We couldn&#39;t process<br>your payment
                                            </h1>
                                            <p style="margin:0 0 28px;font-size:15px;color:#444444;line-height:1.7;">
                                              Hi <strong>{customerName}</strong>, unfortunately the payment for your order
                                              could not be completed. Your order is still saved — please update your payment
                                              details and try again.
                                            </p>

                                            <table width="100%" cellpadding="0" cellspacing="0"
                                                   style="background:#fff5f5;border:1px solid #fecaca;border-radius:8px;margin-bottom:28px;">
                                              <tr>
                                                <td style="padding:20px 24px;">
                                                  <table width="100%" cellpadding="0" cellspacing="0">
                                                    <tr>
                                                      <td>
                                                        <p style="margin:0 0 4px;font-size:12px;color:#888888;
                                                                   text-transform:uppercase;letter-spacing:1px;">Order ID</p>
                                                        <p style="margin:0;font-size:14px;font-weight:600;color:{GradientStart};
                                                                   font-family:'Courier New',monospace;">{orderId}</p>
                                                      </td>
                                                      <td align="right">
                                                        <p style="margin:0 0 4px;font-size:12px;color:#888888;
                                                                   text-transform:uppercase;letter-spacing:1px;">Amount</p>
                                                        <p style="margin:0;font-size:20px;font-weight:700;color:{ErrorColor};">{amount}</p>
                                                      </td>
                                                    </tr>
                                                  </table>
                                                </td>
                                              </tr>
                                            </table>

                                            <table width="100%" cellpadding="0" cellspacing="0"
                                                   style="background:#f8f9fa;border:1px solid #e9ecef;border-radius:8px;margin-bottom:32px;">
                                              <tr>
                                                <td style="padding:14px 20px;">
                                                  <table cellpadding="0" cellspacing="0"><tr>
                                                    <td style="font-size:18px;padding-right:10px;color:{ErrorColor};">&#9888;</td>
                                                    <td>
                                                      <p style="margin:0 0 2px;font-size:12px;color:#888888;
                                                                 text-transform:uppercase;letter-spacing:1px;">Reason</p>
                                                      <p style="margin:0;font-size:14px;color:#444444;">{failureReason}</p>
                                                    </td>
                                                  </tr></table>
                                                </td>
                                              </tr>
                                            </table>

                                            <table width="100%" cellpadding="0" cellspacing="0"
                                                   style="border-top:1px solid #f0f0f0;">
                                              <tr>
                                                <td style="padding-top:20px;">
                                                  <p style="margin:0;font-size:13px;color:#888888;line-height:1.6;">
                                                    Common reasons for failure include incorrect card details, an expired card,
                                                    or a temporary block from your bank. If the problem persists, please
                                                    contact your card issuer or our support team.
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

        public static string PasswordChanged(
            string customerName,
            string changeDate,
            string ipAddress,
            string resetLink,
            string privacyPolicyLink)
        {
            string ipRow = string.IsNullOrWhiteSpace(ipAddress) ? "" : $"""
                                                    <tr>
                                                      <td style="padding:8px 0;font-size:13px;color:#888888;
                                                                 border-bottom:1px solid #e9ecef;">IP Address</td>
                                                      <td align="right" style="padding:8px 0;font-size:13px;color:#444444;
                                                                               font-family:'Courier New',monospace;
                                                                               border-bottom:1px solid #e9ecef;">{ipAddress}</td>
                                                    </tr>
                              """;

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
                                                <span style="color:{GradientStart};">Order</span><span style="color:{GradientEnd};">Flow</span>
                                              </td>
                                              <td align="right" style="font-size:11px;color:#555555;letter-spacing:1.5px;text-transform:uppercase;">
                                                Security Alert
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
                                            <h1 style="margin:0 0 16px;font-size:28px;color:#0a0a0a;font-weight:700;
                                                        line-height:1.2;font-family:Georgia,serif;">
                                              Your password<br>was changed
                                            </h1>
                                            <p style="margin:0 0 28px;font-size:15px;color:#444444;line-height:1.7;">
                                              Hi <strong>{customerName}</strong>, the password for your <strong>OrderFlow</strong>
                                              account was successfully updated. If this was you, no further action is needed.
                                            </p>

                                            <table width="100%" cellpadding="0" cellspacing="0"
                                                   style="background:#f8f9fa;border:1px solid #e9ecef;border-radius:8px;margin-bottom:28px;">
                                              <tr>
                                                <td style="padding:20px 24px;">
                                                  <table width="100%" cellpadding="0" cellspacing="0">
                                                    <tr>
                                                      <td style="padding:8px 0;font-size:13px;color:#888888;
                                                                 border-bottom:1px solid #e9ecef;">Event</td>
                                                      <td align="right" style="padding:8px 0;font-size:13px;font-weight:600;
                                                                               color:#0a0a0a;border-bottom:1px solid #e9ecef;">Password Changed</td>
                                                    </tr>
                                                    <tr>
                                                      <td style="padding:8px 0;font-size:13px;color:#888888;
                                                                 border-bottom:1px solid #e9ecef;">Date &amp; Time</td>
                                                      <td align="right" style="padding:8px 0;font-size:13px;color:#444444;
                                                                               border-bottom:1px solid #e9ecef;">{changeDate}</td>
                                                    </tr>
                                                    {ipRow}
                                                  </table>
                                                </td>
                                              </tr>
                                            </table>

                                            <table width="100%" cellpadding="0" cellspacing="0"
                                                   style="background:#fff8f0;border:1px solid #fde3c0;border-radius:8px;margin-bottom:32px;">
                                              <tr>
                                                <td style="padding:16px 20px;">
                                                  <table cellpadding="0" cellspacing="0"><tr>
                                                    <td style="font-size:20px;padding-right:12px;color:#e67e00;">&#9888;</td>
                                                    <td style="font-size:14px;color:#444444;line-height:1.6;">
                                                      <strong style="color:#0a0a0a;">Didn&#39;t make this change?</strong><br>
                                                      If you did not change your password, your account may be compromised.
                                                      Reset it immediately using the button below.
                                                    </td>
                                                  </tr></table>
                                                </td>
                                              </tr>
                                            </table>

                                            <table cellpadding="0" cellspacing="0" style="margin:0 0 16px;">
                                              <tr>
                                                <td style="background:linear-gradient(to right,{GradientStart},{GradientEnd});
                                                           border-radius:8px;box-shadow:0 0.5rem 1rem rgba(0,0,0,0.15);">
                                                  <a href="{resetLink}"
                                                     style="display:block;padding:16px 44px;color:#ffffff;text-decoration:none;
                                                            font-size:15px;font-weight:600;letter-spacing:0.3px;">
                                                    Secure My Account &#8594;
                                                  </a>
                                                </td>
                                              </tr>
                                            </table>

                                            <table width="100%" cellpadding="0" cellspacing="0"
                                                   style="border-top:1px solid #f0f0f0;">
                                              <tr>
                                                <td style="padding-top:20px;">
                                                  <p style="margin:0;font-size:13px;color:#888888;line-height:1.6;">
                                                    For your security, OrderFlow will never ask for your password by email or phone.
                                                    If you&#39;re unsure about any account activity, please contact our support team.
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

        public static string ConfirmEmail(string confirmLink, string privacyPolicyLink)
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
                                                <span style="color:{GradientStart};">Order</span><span style="color:{GradientEnd};">Flow</span>
                                              </td>
                                              <td align="right" style="font-size:11px;color:#555555;letter-spacing:1.5px;text-transform:uppercase;">
                                                Confirm Email
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
                                                       text-transform:uppercase;color:{GradientStart};">Email Verification</p>
                                            <h1 style="margin:0 0 20px;font-size:28px;color:#0a0a0a;font-weight:700;
                                                        line-height:1.2;font-family:Georgia,serif;">
                                              Confirm your<br>email address
                                            </h1>
                                            <p style="margin:0 0 28px;font-size:15px;color:#444444;line-height:1.7;">
                                              Thank you for signing up for <strong>OrderFlow</strong>. Click the button below
                                              to verify your email address and activate your account. This link will expire
                                              in <strong>2 hours</strong>.
                                            </p>

                                            <table cellpadding="0" cellspacing="0" style="margin:0 0 32px;">
                                              <tr>
                                                <td style="background:linear-gradient(to right,{GradientStart},{GradientEnd});
                                                           border-radius:8px;box-shadow:0 0.5rem 1rem rgba(0,0,0,0.15);">
                                                  <a href="{confirmLink}"
                                                     style="display:block;padding:16px 44px;color:#ffffff;text-decoration:none;
                                                            font-size:15px;font-weight:600;letter-spacing:0.3px;">
                                                    Confirm Email &#8594;
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
                                                             font-family:'Courier New',monospace;">{confirmLink}</p>
                                                </td>
                                              </tr>
                                            </table>

                                            <table width="100%" cellpadding="0" cellspacing="0"
                                                   style="border-top:1px solid #f0f0f0;">
                                              <tr>
                                                <td style="padding-top:20px;">
                                                  <p style="margin:0;font-size:13px;color:#888888;line-height:1.6;">
                                                    If you didn&#39;t create an OrderFlow account, you can safely ignore this
                                                    email &#8212; no account will be activated. If you&#39;re concerned about
                                                    your account&#39;s security, please contact our support team.
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