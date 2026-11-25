using Microsoft.Data.SqlClient;

const string CONN_STR = "Server=.;TrustServerCertificate=True;Database=EHODSI_DB;Integrated Security=True";
const decimal SEUIL_BOURSE = 50000m;

// Étapes 1 & 2 : affichage initial des étudiants avec bourse > 50 000, triés par ordre décroissant
Console.WriteLine("=== Étudiants (MtBourse > 50 000) ===");
AfficherFiltres();

// Menu CRUD principal
bool running = true;
while (running)
{
    Console.WriteLine("\n[C]réer  [R]echercher  [U]pdater  [D]életer  [L]ister  [Q]uitter");
    Console.Write("Choix : ");
    char choix = char.ToUpper(Console.ReadKey().KeyChar);
    Console.WriteLine();

    switch (choix)
    {
        case 'C': Creer();    break;
        case 'R': Lire();     break;
        case 'U': Modifier(); break;
        case 'D': Supprimer(); break;
        case 'L': ListerTous(); break;
        case 'Q': running = false; break;
        default:  Console.WriteLine("Option invalide."); break;
    }
}

// Affiche les étudiants dont la bourse dépasse le seuil, avec date de naissance (étapes 1 & 2)
void AfficherFiltres()
{
    const string sql = "SELECT NomEtudiant, MtBourse, DateNaissance FROM Etudiants WHERE MtBourse > @bourse ORDER BY MtBourse DESC";
    using var conn = new SqlConnection(CONN_STR);
    using var cmd  = new SqlCommand(sql, conn);
    cmd.Parameters.AddWithValue("@bourse", SEUIL_BOURSE);
    try
    {
        conn.Open();
        using var reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            string date = reader.IsDBNull(2) ? "" : ((DateTime)reader[2]).ToString("dd/MM/yyyy");
            Console.WriteLine($"\t{reader[0],-20}{reader[1],10}   {date}");
        }
    }
    catch (Exception ex) { Console.WriteLine(ex.Message); }
}

// Liste tous les étudiants avec leur numéro
void ListerTous()
{
    const string sql = "SELECT Numero, NomEtudiant, MtBourse, DateNaissance FROM Etudiants ORDER BY Numero";
    using var conn = new SqlConnection(CONN_STR);
    using var cmd  = new SqlCommand(sql, conn);
    try
    {
        conn.Open();
        using var reader = cmd.ExecuteReader();
        Console.WriteLine($"\n{"N°",-4} {"Nom",-22} {"Bourse",10}  {"Date Naissance"}");
        Console.WriteLine(new string('-', 55));
        while (reader.Read())
        {
            string date = reader.IsDBNull(3) ? "-" : ((DateTime)reader[3]).ToString("dd/MM/yyyy");
            Console.WriteLine($"{reader[0],-4} {reader[1],-22} {reader[2],10}  {date}");
        }
    }
    catch (Exception ex) { Console.WriteLine(ex.Message); }
}

void Creer()
{
    Console.Write("Nom : ");
    string nom = Console.ReadLine() ?? "";
    Console.Write("Bourse : ");
    if (!decimal.TryParse(Console.ReadLine(), out decimal bourse)) { Console.WriteLine("Montant invalide."); return; }
    Console.Write("Date de naissance (JJ/MM/AAAA, optionnel) : ");
    string dateStr = Console.ReadLine() ?? "";
    DateTime? dateNaiss = DateTime.TryParseExact(dateStr, "dd/MM/yyyy", null,
        System.Globalization.DateTimeStyles.None, out var d) ? d : null;

    const string sql = "INSERT INTO Etudiants (NomEtudiant, MtBourse, DateNaissance) VALUES (@nom, @bourse, @date)";
    using var conn = new SqlConnection(CONN_STR);
    using var cmd  = new SqlCommand(sql, conn);
    cmd.Parameters.AddWithValue("@nom",    nom);
    cmd.Parameters.AddWithValue("@bourse", bourse);
    cmd.Parameters.AddWithValue("@date",   (object?)dateNaiss ?? DBNull.Value);
    try
    {
        conn.Open();
        Console.WriteLine(cmd.ExecuteNonQuery() > 0 ? "Étudiant ajouté." : "Échec.");
    }
    catch (Exception ex) { Console.WriteLine(ex.Message); }
}

void Lire()
{
    Console.Write("Numéro : ");
    if (!int.TryParse(Console.ReadLine(), out int num)) { Console.WriteLine("Numéro invalide."); return; }

    const string sql = "SELECT Numero, NomEtudiant, MtBourse, DateNaissance FROM Etudiants WHERE Numero = @num";
    using var conn = new SqlConnection(CONN_STR);
    using var cmd  = new SqlCommand(sql, conn);
    cmd.Parameters.AddWithValue("@num", num);
    try
    {
        conn.Open();
        using var reader = cmd.ExecuteReader();
        if (reader.Read())
        {
            string date = reader.IsDBNull(3) ? "-" : ((DateTime)reader[3]).ToString("dd/MM/yyyy");
            Console.WriteLine($"N°{reader[0]}  {reader[1]}  {reader[2]} F  Naissance : {date}");
        }
        else Console.WriteLine("Étudiant introuvable.");
    }
    catch (Exception ex) { Console.WriteLine(ex.Message); }
}

// La modification peut porter sur le nom et/ou le montant de la bourse
void Modifier()
{
    Console.Write("Numéro de l'étudiant à modifier : ");
    if (!int.TryParse(Console.ReadLine(), out int num)) { Console.WriteLine("Numéro invalide."); return; }

    Console.Write("Nouveau nom (vide = inchangé) : ");
    string nom = Console.ReadLine() ?? "";
    Console.Write("Nouveau montant bourse (vide = inchangé) : ");
    string bourseStr = Console.ReadLine() ?? "";

    var parts = new List<string>();
    using var conn = new SqlConnection(CONN_STR);
    using var cmd  = new SqlCommand("", conn);

    if (!string.IsNullOrWhiteSpace(nom))
    {
        parts.Add("NomEtudiant = @nom");
        cmd.Parameters.AddWithValue("@nom", nom);
    }
    if (decimal.TryParse(bourseStr, out decimal bourse))
    {
        parts.Add("MtBourse = @bourse");
        cmd.Parameters.AddWithValue("@bourse", bourse);
    }

    if (parts.Count == 0) { Console.WriteLine("Aucune modification."); return; }

    cmd.CommandText = $"UPDATE Etudiants SET {string.Join(", ", parts)} WHERE Numero = @num";
    cmd.Parameters.AddWithValue("@num", num);
    try
    {
        conn.Open();
        Console.WriteLine(cmd.ExecuteNonQuery() > 0 ? "Modifié." : "Introuvable.");
    }
    catch (Exception ex) { Console.WriteLine(ex.Message); }
}

// Demande confirmation avant suppression définitive
void Supprimer()
{
    Console.Write("Numéro de l'étudiant à supprimer : ");
    if (!int.TryParse(Console.ReadLine(), out int num)) { Console.WriteLine("Numéro invalide."); return; }

    using var conn = new SqlConnection(CONN_STR);
    try
    {
        conn.Open();

        var cmdNom = new SqlCommand("SELECT NomEtudiant FROM Etudiants WHERE Numero = @num", conn);
        cmdNom.Parameters.AddWithValue("@num", num);
        string? nom = cmdNom.ExecuteScalar()?.ToString();

        if (nom == null) { Console.WriteLine("Étudiant introuvable."); return; }

        Console.Write($"Supprimer '{nom}' ? (O/N) : ");
        if (char.ToUpper(Console.ReadKey().KeyChar) != 'O') { Console.WriteLine("\nAnnulé."); return; }
        Console.WriteLine();

        var cmdDel = new SqlCommand("DELETE FROM Etudiants WHERE Numero = @num", conn);
        cmdDel.Parameters.AddWithValue("@num", num);
        Console.WriteLine(cmdDel.ExecuteNonQuery() > 0 ? "Supprimé." : "Échec.");
    }
    catch (Exception ex) { Console.WriteLine(ex.Message); }
}
