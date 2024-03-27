﻿namespace BlazorECommerce.Server.Data;

public class DataContext : DbContext
{
    public DataContext(DbContextOptions<DataContext> options) : base(options)
    {
        
    }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Product>().HasData(
             new()
             {
                 Id = 1,
                 Title = "The Hitchhiker's Guide to the Galaxy",
                 Description = "The Hitchhiker's Guide to the Galaxy is an international multimedia phenomenon; the novels are the most widely distributed, having been translated into more than 30 languages by 2005.[4][5] The first novel, The Hitchhiker's Guide to the Galaxy (1979), has been ranked fourth on the BBC's The Big Read poll.[6] The sixth novel, And Another Thing..., was written by Eoin Colfer with additional unpublished material by Douglas Adams. In 2017, BBC Radio 4 announced a 40th-anniversary celebration with Dirk Maggs, one of the original producers, in charge.[7] The first of six new episodes was broadcast on 8 March 2018.[",
                 ImgUrl = "https://upload.wikimedia.org/wikipedia/en/b/bd/H2G2_UK_front_cover.jpg",
                 Price = 9.99m
             },
             new()
             {
                 Id = 2,
                 Title = "Ready Player One",
                 Description = "Ready Player One is a 2011 science fiction novel, and the debut novel of American author Ernest Cline. The story, set in a dystopia in 2045, follows protagonist Wade Watts on his search for an Easter egg in a worldwide virtual reality game, the discovery of which would lead him to inherit the game creator's fortune. Cline sold the rights to publish the novel in June 2010, in a bidding war to the Crown Publishing Group (a division of Random House).[1] The book was published on August 16, 2011.[2] An audiobook was released the same day; it was narrated by Wil Wheaton, who was mentioned briefly in one of the chapters.[3][4]Ch. 20 In 2012, the book received an Alex Award from the Young Adult Library Services Association division of the American Library Association[5] and won the 2011 Prometheus Award.",
                 ImgUrl = "https://upload.wikimedia.org/wikipedia/en/thumb/a/a4/Ready_Player_One_cover.jpg/220px-Ready_Player_One_cover.jpg",
                 Price = 7.99m
             },
             new()
             {
                 Id = 3,
                 Title = "Nineteen Eighty-Four",
                 Description = "Nineteen Eighty-Four (also published as 1984) is a dystopian novel and cautionary tale by English writer George Orwell. It was published on 8 June 1949 by Secker & Warburg as Orwell's ninth and final book completed in his lifetime. Thematically, it centres on the consequences of totalitarianism, mass surveillance, and repressive regimentation of people and behaviours within society.[3][4] Orwell, a democratic socialist, modelled the authoritarian state in the novel on the Soviet Union in the era of Stalinism and Nazi Germany.[5] More broadly, the novel examines the role of truth and facts within societies and the ways in which they can be manipulated.",
                 ImgUrl = "https://upload.wikimedia.org/wikipedia/en/5/51/1984_first_edition_cover.jpg",
                 Price = 8.99m
             },
              new()
              {
                  Id = 4,
                  Title = "Fifty Shades of Grey",
                  Description = "Fifty Shades of Grey is a 2011 erotic romance novel by British author E. L. James.[1] It became the first instalment in the Fifty Shades novel series that follows the deepening relationship between a college graduate, Anastasia Steele, and a young business magnate, Christian Grey. It contains explicitly erotic scenes featuring elements of sexual practices involving BDSM (bondage/discipline, dominance/submission, and sadism/masochism).",
                  ImgUrl = "https://upload.wikimedia.org/wikipedia/en/thumb/5/5e/50ShadesofGreyCoverArt.jpg/220px-50ShadesofGreyCoverArt.jpg",
                  Price = 2.64m
              }
        );
    }

    public DbSet<Product> Products{ get; set; }
}
