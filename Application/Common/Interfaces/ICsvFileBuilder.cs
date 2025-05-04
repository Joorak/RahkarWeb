namespace Application.Common.Interfaces; 
public interface ICsvFileBuilder {
    byte[] BuildCsvFile<T>(IEnumerable<T> records);
} 
