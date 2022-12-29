//Hãy bật tab đánh giá giảng viên sau đó mới chạy code này !
function rateAll(){
      rate = {}
      document.querySelectorAll('input').forEach(function(input){
            rate[parseInt(input.id.split('_')[0])] = parseInt(input.id.split('_')[1])
      })
      for(var id_rate in rate){
            if(isNaN(id_rate) == false){
                  document.getElementById(+id_rate+"_"+rate[id_rate]).checked = true
            }
      }
}
console.log("Đã cài đặt hàm auto đánh giá rateAll thành công !")