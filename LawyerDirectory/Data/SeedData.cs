using LawyerDirectory.Models;
using Microsoft.EntityFrameworkCore;

namespace LawyerDirectory.Data
{
    public static class SeedData
    {
        public static void Initialize(IServiceProvider serviceProvider)
        {
            using (var context = new LawyerContext(
                serviceProvider.GetRequiredService<DbContextOptions<LawyerContext>>()))
            {
                // Check if database already has data
                if (context.Lawyers.Any())
                {
                    return;   // Database has been seeded
                }

                context.Lawyers.AddRange(
                    // California Lawyers
                    new Lawyer
                    {
                        Name = "Sarah Johnson",
                        ProfilePictureUrl = "https://i.pravatar.cc/150?img=1",
                        AreaOfPractice = "Corporate Law, Mergers & Acquisitions",
                        Description = "Specializes in complex corporate transactions and has advised on over $5 billion in M&A deals.",
                        YearsOfExperience = 15,
                        State = "California",
                        City = "San Francisco",
                        Website = "https://www.sklawfirm.com",
                        AmlawRanking = "50"
                    },
                    new Lawyer
                    {
                        Name = "Michael Chen",
                        ProfilePictureUrl = "https://i.pravatar.cc/150?img=2",
                        AreaOfPractice = "Intellectual Property, Patent Law",
                        Description = "Patent attorney with expertise in technology and software patents. Former software engineer.",
                        YearsOfExperience = 10,
                        State = "California",
                        City = "San Jose",
                        Website = "https://www.chenlawgroup.com",
                        AmlawRanking = "NR"
                    },
                    new Lawyer
                    {
                        Name = "Jennifer Martinez",
                        ProfilePictureUrl = "https://i.pravatar.cc/150?img=5",
                        AreaOfPractice = "Employment Law, Labor Relations",
                        Description = "Represents both employers and employees in complex employment disputes and labor negotiations.",
                        YearsOfExperience = 12,
                        State = "California",
                        City = "Los Angeles",
                        Website = "https://www.martinezlabor.com",
                        AmlawRanking = "NR"
                    },
                    new Lawyer
                    {
                        Name = "David Kim",
                        ProfilePictureUrl = "https://i.pravatar.cc/150?img=3",
                        AreaOfPractice = "Real Estate Law, Commercial Transactions",
                        Description = "Handles commercial real estate transactions, leasing, and property development.",
                        YearsOfExperience = 8,
                        State = "California",
                        City = "San Diego",
                        Website = "https://www.kimrealestate.com",
                        AmlawRanking = "NR"
                    },

                    // New York Lawyers
                    new Lawyer
                    {
                        Name = "Robert Williams",
                        ProfilePictureUrl = "https://i.pravatar.cc/150?img=4",
                        AreaOfPractice = "Securities Law, Financial Regulation",
                        Description = "Former SEC attorney specializing in securities offerings, compliance, and enforcement defense.",
                        YearsOfExperience = 20,
                        State = "New York",
                        City = "New York",
                        Website = "https://www.williamssec.com",
                        AmlawRanking = "25"
                    },
                    new Lawyer
                    {
                        Name = "Emily Rodriguez",
                        ProfilePictureUrl = "https://i.pravatar.cc/150?img=6",
                        AreaOfPractice = "Litigation, Commercial Disputes",
                        Description = "Trial attorney with extensive experience in complex commercial litigation and arbitration.",
                        YearsOfExperience = 14,
                        State = "New York",
                        City = "New York",
                        Website = "https://www.rodriguezlitigation.com",
                        AmlawRanking = "100"
                    },
                    new Lawyer
                    {
                        Name = "James Thompson",
                        ProfilePictureUrl = "https://i.pravatar.cc/150?img=7",
                        AreaOfPractice = "Tax Law, Estate Planning",
                        Description = "Provides tax planning and estate planning services for high-net-worth individuals and families.",
                        YearsOfExperience = 18,
                        State = "New York",
                        City = "Buffalo",
                        Website = "https://www.thompsontax.com",
                        AmlawRanking = "NR"
                    },
                    new Lawyer
                    {
                        Name = "Lisa Anderson",
                        ProfilePictureUrl = "https://i.pravatar.cc/150?img=8",
                        AreaOfPractice = "Immigration Law",
                        Description = "Assists individuals and corporations with visa applications, green cards, and citizenship matters.",
                        YearsOfExperience = 7,
                        State = "New York",
                        City = "Rochester",
                        Website = "https://www.andersonimmigration.com",
                        AmlawRanking = "NR"
                    },

                    // Texas Lawyers
                    new Lawyer
                    {
                        Name = "William Brown",
                        ProfilePictureUrl = "https://i.pravatar.cc/150?img=9",
                        AreaOfPractice = "Energy Law, Oil & Gas",
                        Description = "Represents energy companies in regulatory matters, transactions, and disputes.",
                        YearsOfExperience = 22,
                        State = "Texas",
                        City = "Houston",
                        Website = "https://www.brownenergy.com",
                        AmlawRanking = "75"
                    },
                    new Lawyer
                    {
                        Name = "Maria Garcia",
                        ProfilePictureUrl = "https://i.pravatar.cc/150?img=10",
                        AreaOfPractice = "Family Law, Divorce",
                        Description = "Compassionate representation in divorce, child custody, and family law matters.",
                        YearsOfExperience = 11,
                        State = "Texas",
                        City = "Austin",
                        Website = "https://www.garciafamilylaw.com",
                        AmlawRanking = "NR"
                    },
                    new Lawyer
                    {
                        Name = "Christopher Davis",
                        ProfilePictureUrl = "https://i.pravatar.cc/150?img=11",
                        AreaOfPractice = "Criminal Defense, White Collar Crime",
                        Description = "Former federal prosecutor defending individuals and corporations in criminal investigations.",
                        YearsOfExperience = 16,
                        State = "Texas",
                        City = "Dallas",
                        Website = "https://www.davisdefense.com",
                        AmlawRanking = "NR"
                    },
                    new Lawyer
                    {
                        Name = "Amanda Wilson",
                        ProfilePictureUrl = "https://i.pravatar.cc/150?img=12",
                        AreaOfPractice = "Healthcare Law, Regulatory Compliance",
                        Description = "Advises healthcare providers on regulatory compliance, licensing, and reimbursement issues.",
                        YearsOfExperience = 9,
                        State = "Texas",
                        City = "San Antonio",
                        Website = "https://www.wilsonhealthlaw.com",
                        AmlawRanking = "NR"
                    },

                    // Florida Lawyers
                    new Lawyer
                    {
                        Name = "John Miller",
                        ProfilePictureUrl = "https://i.pravatar.cc/150?img=13",
                        AreaOfPractice = "Personal Injury, Medical Malpractice",
                        Description = "Represents victims of negligence and medical malpractice in complex personal injury cases.",
                        YearsOfExperience = 25,
                        State = "Florida",
                        City = "Miami",
                        Website = "https://www.millerpersonalinjury.com",
                        AmlawRanking = "NR"
                    },
                    new Lawyer
                    {
                        Name = "Patricia Moore",
                        ProfilePictureUrl = "https://i.pravatar.cc/150?img=14",
                        AreaOfPractice = "Environmental Law, Regulatory Compliance",
                        Description = "Assists clients with environmental permitting, compliance, and litigation.",
                        YearsOfExperience = 13,
                        State = "Florida",
                        City = "Tampa",
                        Website = "https://www.mooreenviro.com",
                        AmlawRanking = "NR"
                    },
                    new Lawyer
                    {
                        Name = "Thomas Taylor",
                        ProfilePictureUrl = "https://i.pravatar.cc/150?img=15",
                        AreaOfPractice = "Bankruptcy Law, Debt Restructuring",
                        Description = "Represents businesses and individuals in bankruptcy proceedings and debt restructuring.",
                        YearsOfExperience = 17,
                        State = "Florida",
                        City = "Orlando",
                        Website = "https://www.taylorbankruptcy.com",
                        AmlawRanking = "NR"
                    },

                    // Illinois Lawyers
                    new Lawyer
                    {
                        Name = "Daniel Anderson",
                        ProfilePictureUrl = "https://i.pravatar.cc/150?img=16",
                        AreaOfPractice = "Antitrust Law, Competition Law",
                        Description = "Counsels clients on antitrust compliance and defends against antitrust investigations.",
                        YearsOfExperience = 19,
                        State = "Illinois",
                        City = "Chicago",
                        Website = "https://www.andersonantitrust.com",
                        AmlawRanking = "40"
                    },
                    new Lawyer
                    {
                        Name = "Susan White",
                        ProfilePictureUrl = "https://i.pravatar.cc/150?img=17",
                        AreaOfPractice = "Education Law, Civil Rights",
                        Description = "Represents students, parents, and educational institutions in civil rights and education matters.",
                        YearsOfExperience = 10,
                        State = "Illinois",
                        City = "Springfield",
                        Website = "https://www.whiteeducationlaw.com",
                        AmlawRanking = "NR"
                    },
                    new Lawyer
                    {
                        Name = "Mark Harris",
                        ProfilePictureUrl = "https://i.pravatar.cc/150?img=18",
                        AreaOfPractice = "Construction Law, Contract Disputes",
                        Description = "Handles construction disputes, mechanic's liens, and contract negotiations.",
                        YearsOfExperience = 12,
                        State = "Illinois",
                        City = "Chicago",
                        Website = "https://www.harrisconstruction.com",
                        AmlawRanking = "NR"
                    },

                    // Massachusetts Lawyers
                    new Lawyer
                    {
                        Name = "Barbara Martin",
                        ProfilePictureUrl = "https://i.pravatar.cc/150?img=19",
                        AreaOfPractice = "Biotechnology Law, Life Sciences",
                        Description = "Advises biotech and pharmaceutical companies on licensing, IP, and regulatory matters.",
                        YearsOfExperience = 14,
                        State = "Massachusetts",
                        City = "Boston",
                        Website = "https://www.martinbiotech.com",
                        AmlawRanking = "60"
                    },
                    new Lawyer
                    {
                        Name = "Kevin Lee",
                        ProfilePictureUrl = "https://i.pravatar.cc/150?img=20",
                        AreaOfPractice = "Privacy Law, Data Security",
                        Description = "Helps organizations comply with privacy laws and respond to data breaches.",
                        YearsOfExperience = 6,
                        State = "Massachusetts",
                        City = "Cambridge",
                        Website = "https://www.leeprivacy.com",
                        AmlawRanking = "NR"
                    }
                );

                context.SaveChanges();
            }
        }
    }
}
