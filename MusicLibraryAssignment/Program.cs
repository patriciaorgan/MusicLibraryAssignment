using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

namespace MusicLibraryAssignment
{
    class Program
    {
        static void Main(string[] args)
        {
            MySqlConnection con = new MySqlConnection("Server=mysqlmusiclibrary.cby8b2letu5u.eu-west-1.rds.amazonaws.com; Port=3360; Database=MusicDB; Uid=porgan; Pwd=popassword;");
          
           MySqlDataReader rdr = null;
            string addstring1 = @"insert into Artist
                                (ArtistName)
                                values('Bob Dylan')";
            string addstring2 = @"insert into Album
                                (AlbumTitle)
                                values('Top Hits 2015')";
            string addstring3 = @"insert into song
                                (songName,songLength,idArtist,idAlbum)
                                values('Blowin'' in the Wind','2.42', 
                                (select idArtist 
                                from Artist 
                                where ArtistName ='Bob Dylan' AND idArtist = 1),
                                (select idAlbum 
                                from Album 
                                where AlbumTitle ='Top Hits 2015' AND idAlbum = 1))";
            string readerstring = @"select 
                                s.idsong, s.songName, s.songLength, ar.ArtistName, ab.AlbumTitle, s.songGenre 
                                from song AS s, Artist AS ar, Album AS ab
                                where s.idArtist = ar.idArtist AND s.idAlbum= ab.idAlbum order by s.idsong ASC";
            string readArtist = @"select * from Artist order by idArtist ASC";
            string readAlbum =  @"select * from Album order by idAlbum ASC";

            try
            {
                //open connection
                con.Open();
                //create a command object, set it insially to first string
                MySqlCommand cmd = new MySqlCommand(readerstring,con);
                //as duplicates are not allowed can do a check to see when a new row is 
                //wanted to be added that it doesnt already exist
                Console.WriteLine("Please type a new artist name: ");
                string artist = Console.ReadLine();


                while (!addArtist(cmd, artist))
                {

                    Console.WriteLine("This Artist already exists, enter another please, "+
                                        "or hit enter to continue and use your first choice");
                    string nextartist = Console.ReadLine();
                    if (nextartist.Length == 0)
                    {
                        break;
                    }
                    artist = nextartist;

                }//end while loop
 

                Console.WriteLine("Enter new Album name");
                string album = Console.ReadLine();
                while (!addAlbum(cmd, album))
                {

                    Console.WriteLine("This Album already exists, enter another please," +
                                     " or hit enter to continue, and use your first choice");
                    string nextalbum = Console.ReadLine();
                    if (nextalbum.Length == 0)
                    {
                        break;
                    }
                    album = nextalbum;

                }//end while loop

                Console.WriteLine("Enter new Song Name");
                string song = Console.ReadLine();
                Console.WriteLine("Enter new Song Length");
                string songLnt = Console.ReadLine();
                Console.WriteLine("Enter new Song Genre");
                string songGenre = Console.ReadLine();
                while (!addSong(cmd, song, songLnt, songGenre, artist, album))
                {

                    Console.WriteLine("This Song already exists, enter another Song Name followed by enter " +
                                "\n and then Song length and enter and"+
                                "\nSong Genre and Enter please, or hit enter to cencel");
                    string nextsong = Console.ReadLine();
                    string nextsongLnt = Console.ReadLine();
                    string nextsongGenre = Console.ReadLine();

                    if (nextsong.Length == 0)
                    {
                        break;
                    }

                }//end while loop


               /*
                //execute with theis first string for insert
                cmd.ExecuteNonQuery();
                //change the command text to the other strings and execute each
                cmd.CommandText = addstring2;
                cmd.ExecuteNonQuery();
                cmd.CommandText = addstring3;
                cmd.ExecuteNonQuery();
                */
                if (rdr != null)
                {
                    rdr.Close();
                }
                //set command text to the query for the  reader object to call back all from the 3 tables in one query
                cmd.CommandText = readerstring;
                
                rdr = cmd.ExecuteReader();

                //loop through and print results
                Console.WriteLine("\nConcatinated Query\n");
                while (rdr.Read())
                {
                    Console.WriteLine( rdr[0] + ": " + rdr[1] + ", " + rdr[2] + ", " + rdr[3] + ", " + rdr[4] +", " + rdr[5]);
                }
                //need to close reader before using it again
                rdr.Close();

                cmd.CommandText = readArtist;
                rdr = cmd.ExecuteReader();

                //loop through and print results
                Console.WriteLine("\nArtist Table\n");
                while (rdr.Read())
                {
                    Console.WriteLine( rdr[0] + ": " + rdr[1]);
                }
                //need to close reader before using it again
                rdr.Close();

                cmd.CommandText = readAlbum;
                rdr = cmd.ExecuteReader();

                //loop through and print results
                Console.WriteLine("\nAlbum Table\n");
                while (rdr.Read())
                {
                    Console.WriteLine( rdr[0] + ": " + rdr[1]);
                }
            }
            catch(MySqlException mye)
            {
                Console.WriteLine("Error with database: " + mye);
            }
            finally
            {
                if (rdr == null)
                {
                    Console.WriteLine("nothing to read");
                }
                else if (rdr != null)
                {
                    rdr.Close();
                }
                else if (con != null)
                {
                    con.Close();
                }
                Console.ReadKey();
            }

        }//end Main method

        static Boolean addArtist(MySqlCommand command, string input)
        {
            try
            {
                string inputtext = @"Select ArtistName from Artist
                                where ArtistName ='" + input+ "'";
                command.CommandText = inputtext;
                if (input == (string)command.ExecuteScalar())
                {
                    return false;
                }
                else
                {
                    //if it does not already exist in table it can be added
                    string addstring1 = @"insert into Artist
                                (ArtistName)
                                values('" + input +"')";
                    command.CommandText = addstring1;
                    command.ExecuteNonQuery();
                    return true;
                }
            }
            catch (MySqlException mye)
            {
                Console.WriteLine("Error with Album SQL statments: " + mye);
                return false;
            }
        }//end addArtist

        static Boolean addAlbum(MySqlCommand command, string input)
        {
            try
            {
                string inputtext = @"Select AlbumTitle from Album
                                where AlbumTitle ='" + input + "'";
                command.CommandText = inputtext;
                if (input == (string)command.ExecuteScalar())
                {
                    return false;
                }
                else
                {
                    //if it does not already exist in table it can be added
                    string addstring = @"insert into Album
                                (AlbumTitle)
                                values('" + input + "')";
                    command.CommandText = addstring;
                    command.ExecuteNonQuery();
                    return true;
                }
            }
            catch (MySqlException mye)
            {
                Console.WriteLine("Error with Artist SQL statments: " + mye);
                return false;
            }
        }//end addAlbum

        static Boolean addSong(MySqlCommand command, string song, string songlt, string songgen, string art, string alb)
        {
            try
            {
                string inputtext = @"Select songName from song
                                where songName ='" + song + "'";
                command.CommandText = inputtext;
                if (song == (string)command.ExecuteScalar())
                {
                    return false;
                }
                else
                {
                    //if it does not already exist in table it can be added
                    string addstring = @"insert into song
                                (songName, songLength, idArtist, idAlbum, songGenre)
                                values('" + song + "', '" + songlt + "',(Select idArtist from Artist where ArtistName ='" + art+
                                          "'), (Select idAlbum from Album where AlbumTitle = '" + alb + "'),'" + songgen + "')";
                    command.CommandText = addstring;
                    command.ExecuteNonQuery();
                    return true;
                }
            }
            catch (MySqlException mye)
            {
                Console.WriteLine("Error with song SQL statments: " + mye);
                return false;
            }
        }//end addSong method

    }//end Program Class
}//end Namespace
