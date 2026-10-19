namespace INSY7315_Prototype.Services
{
    public class ChatOption
    {
        public string Label { get; set; } = "";
        public string? Query { get; set; }   // sends this as a new question
        public string? Href { get; set; }    // or opens a link (e.g. tel:)
    }

    public class ChatReply
    {
        public string Text { get; set; } = "";
        public List<ChatOption> Options { get; set; } = new();
    }

    public static class ChatbotResponder
    {
        private static ChatOption AskOpt(string label, string query) =>
            new() { Label = label, Query = query };

        private static ChatOption LinkOpt(string label, string href) =>
            new() { Label = label, Href = href };

        public static ChatReply Reply(string? query)
        {
            var q = (query ?? "").Trim().ToLowerInvariant();
            bool Has(params string[] words) => words.Any(w => q.Contains(w));

            // Greeting
            if (q is "hi" or "hello" or "hey" || Has("good morning", "good afternoon"))
            {
                return new ChatReply
                {
                    Text = "Hello! 👋 Welcome to the Bridge & Anchor Assistant.\n\n" +
                           "I can help with loan amounts, repayment terms, required documents, " +
                           "applications, branch locations and contacting our team.",
                    Options =
                    {
                        AskOpt("💰 Loan Information", "How much can I borrow?"),
                        AskOpt("📄 Required Documents", "What documents do I need?"),
                        AskOpt("📝 How to Apply", "How do I apply?")
                    }
                };
            }

            // After applying / status (checked BEFORE "application" so it can be reached)
            if (Has("what happens after", "after i apply", "after applying",
                    "application status", "check my application"))
            {
                return new ChatReply
                {
                    Text = "⏳ After You Apply:\n\n" +
                           "Your application goes through Bridge & Anchor's formal assessment process. " +
                           "You can follow its status on the My Applications page.\n\n" +
                           "If you need more help, please contact Bridge & Anchor staff directly.",
                    Options =
                    {
                        AskOpt("📞 Contact Staff", "How can I contact Bridge and Anchor?"),
                        AskOpt("🏢 Branch Information", "Where are the branches?")
                    }
                };
            }

            // Loan amounts and repayment
            if (Has("how much can i borrow", "loan amount", "borrow", "repayment",
                    "repay", "how long can i repay", "loan terms"))
            {
                return new ChatReply
                {
                    Text = "💰 Loan Amounts & Repayment Terms:\n\n" +
                           "• Loan amounts range from R300 to R7,000.\n" +
                           "• Repayment periods range from 1 to 6 months.\n\n" +
                           "ℹ️ I am an automated support assistant. I cannot make affordability, " +
                           "credit or loan-approval decisions. Applications are assessed through " +
                           "Bridge & Anchor's formal process.",
                    Options =
                    {
                        AskOpt("📄 Required Documents", "What documents do I need?"),
                        AskOpt("📝 How to Apply", "How do I apply?")
                    }
                };
            }

            // Documents
            if (Has("what documents", "required documents", "documents do i need",
                    "loan requirements", "requirements", "payslip", "bank statement",
                    "proof of address"))
            {
                return new ChatReply
                {
                    Text = "📄 Required Documents:\n\n" +
                           "To apply for a loan with Bridge & Anchor you will need:\n\n" +
                           "1. 🆔 South African ID\n" +
                           "2. 🏠 Proof of address\n" +
                           "3. 💵 Payslip\n" +
                           "4. 🏦 3 months bank statements\n\n" +
                           "It is a good idea to have these ready when you apply.",
                    Options =
                    {
                        AskOpt("📝 How to Apply", "How do I apply?"),
                        AskOpt("🏢 Branch Locations", "Where are the branches?")
                    }
                };
            }

            // How to apply
            if (Has("how do i apply", "apply for a loan", "loan application",
                    "application process", "application"))
            {
                return new ChatReply
                {
                    Text = "📝 Loan Application:\n\n" +
                           "Prepare your documents, then use Apply for Loan in your account. " +
                           "You can also visit a Bridge & Anchor branch or contact the team " +
                           "for help with your application.",
                    Options =
                    {
                        new ChatOption { Label = "📝 Apply for Loan", Href = "/Loan/Apply" },
                        AskOpt("🏢 View Branches", "Where are the branches?")
                    }
                };
            }

            // Branches and contact
            if (Has("where are the branches", "branch location", "branch locations",
                    "branch address", "where is the branch", "contact", "phone number",
                    "telephone", "email", "call", "speak to someone", "human support"))
            {
                return new ChatReply
                {
                    Text = "🏢 Bridge & Anchor Branch Information:\n\n" +
                           "📍 Branch 1 – Church Street\n" +
                           "Shop 2, 200 Church Street, Pietermaritzburg, 3201\n" +
                           "📞 Tel: 033 394 3154\n" +
                           "📱 Cell: 076 678 9661\n" +
                           "✉️ Email: Bridgeanchor@gmail.com\n\n" +
                           "📍 Branch 2 – Boshoff Street\n" +
                           "161 Boshoff Street, Pietermaritzburg, 3201\n" +
                           "📞 Tel: 033 345 8021\n" +
                           "📱 Cell: 062 769 2868\n" +
                           "✉️ Email: Bridgeanchor@gmail.com",
                    Options =
                    {
                        LinkOpt("📞 Call Church Street", "tel:0333943154"),
                        LinkOpt("📞 Call Boshoff Street", "tel:0333458021")
                    }
                };
            }

            // Fallback
            return new ChatReply
            {
                Text = "I'm sorry, I couldn't quite understand your question. 😕\n\n" +
                       "I can help with:\n\n" +
                       "• Loan amounts\n• Repayment periods\n• Required documents\n" +
                       "• How to apply\n• What happens after applying\n" +
                       "• Branch locations\n• Contact information",
                Options =
                {
                    AskOpt("📞 Contact Staff", "How can I contact Bridge and Anchor?"),
                    AskOpt("🏢 Branch Information", "Where are the branches?")
                }
            };
        }
    }
}