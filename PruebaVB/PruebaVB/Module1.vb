Imports System.Data.SqlClient
Imports System.IO
Imports System.Security
Imports System.Text.RegularExpressions
Imports System.Xml
Imports System.Xml.Schema
Imports System.Xml.XPath

Module Module1

    Sub Main()
        Try
            ' Simula la entrada del parámetro "cvisor"
            'Console.WriteLine("Introduce el valor del parámetro 'cvisor':")
            Dim cVisor As String = "<Visor><UserId>12345</UserId><Pwd>password123</Pwd><ProyectoId>67890</ProyectoId><DocId>98765</DocId><Llave>ABCDEF123456</Llave><ProcesoId>ABCDEF123456</ProcesoId><PIID>ABCDEF123456</PIID><FlujoID>ABCDEF123456</FlujoID></Visor>"

            ' Validación inicial de cVisor
            If String.IsNullOrEmpty(cVisor) Then
                Throw New Exception("El parámetro enviado está vacío. [cvisor] = " & cVisor)
            End If

            cVisor = cVisor.Replace(" ", "+")
            Dim strXml As String = cVisor ' Simula la decodificación del parámetro

            ' Validar que el XML sea seguro
            Dim patron As New Regex("(<script[^>]*>.*?</script>)|<!\[CDATA\[.*?\]\]>|&.*?;|<!--.*?-->", RegexOptions.IgnoreCase Or RegexOptions.Singleline)

            If Not strXml.StartsWith("<") OrElse Not strXml.EndsWith(">") OrElse patron.IsMatch(strXml) OrElse strXml.Contains("../") OrElse strXml.Contains("..\\") OrElse strXml.Contains("file://") OrElse strXml.Contains("http://") Then
                Throw New Exception("El parámetro [cvisor] no contiene un XML válido.")
            End If

            Dim xsdPath As String = Path.GetFullPath(Path.ChangeExtension(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Archivos/schemaVisor"), ".xsd"))
            Dim settings As New XmlReaderSettings()
            settings.Schemas.Add(String.Empty, xsdPath)
            settings.ValidationType = ValidationType.Schema
            settings.ValidationFlags = XmlSchemaValidationFlags.ReportValidationWarnings
            settings.DtdProcessing = DtdProcessing.Prohibit ' Prohíbe DTDs
            settings.XmlResolver = Nothing ' Evita resolver entidades externas

            ' Expresiones regulares para validar cada nodo del xml
            Dim regexUserId As New Regex("^[a-zA-Z0-9]+$")
            Dim regexPwd As New Regex("^[a-zA-Z0-9+/=]+$")
            Dim regexNumeric As New Regex("^\d+$")

            Using reader As XmlReader = XmlReader.Create(New StringReader(strXml), settings)
                While reader.Read() ' Validar el schema del XML
                    If reader.NodeType = XmlNodeType.Element Then
                        Select Case reader.Name
                            Case "UserId"
                                reader.Read()
                                If Not regexUserId.IsMatch(reader.Value) Then
                                    Throw New Exception($"El valor de 'UserId' no es válido: {reader.Value}")
                                End If

                            Case "Pwd"
                                reader.Read()
                                If Not regexPwd.IsMatch(reader.Value) Then
                                    Throw New Exception($"El valor de 'Pwd' no es válido: {reader.Value}")
                                End If

                            Case "ProyectoId", "DocId", "Llave"
                                reader.Read()
                                If Not regexNumeric.IsMatch(reader.Value) Then
                                    Throw New Exception($"El valor de '{reader.Name}' no es válido: {reader.Value}")
                                End If
                        End Select
                    End If
                End While
            End Using

            'Dim xmlDoc As New XmlDocument()
            'xmlDoc.Load(strXml)
            'Dim d = "C:\Users\KatherineL\OneDrive - Digipro S.A. DE C.V\Documentos/.."

            If Not Directory.Exists(strXml) Then
                Using textReaderXML As TextReader = New StringReader(strXml) 'Ya se valido strXml por lo que es seguro cargarlo como texto y procesar el xml
                    Dim xmlDoc As New XPathDocument(CType(textReaderXML, TextReader))
                    Dim navigator As XPathNavigator = xmlDoc.CreateNavigator()
                    Dim userId As String = SecurityElement.Escape(navigator.SelectSingleNode("/Visor/UserId")?.Value)
                    Dim pwd As String = SecurityElement.Escape(navigator.SelectSingleNode("/Visor/Pwd")?.Value)
                    Dim proyId As String = SecurityElement.Escape(navigator.SelectSingleNode("/Visor/ProyectoId")?.Value)
                    Dim docId As String = SecurityElement.Escape(navigator.SelectSingleNode("/Visor/DocId")?.Value)
                    Dim llave As String = SecurityElement.Escape(navigator.SelectSingleNode("/Visor/Llave")?.Value)



                End Using
            End If
        Catch ex As Exception
            Console.WriteLine("Error: " & ex.Message)

        End Try
    End Sub


    Function DecifrarCadena(input As String) As String
        ' Implementa tu lógica de descifrado aquí
        Return input ' Retorna el mismo valor como ejemplo
    End Function

End Module
